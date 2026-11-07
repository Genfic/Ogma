#!/usr/bin/env bash
set -Eeuo pipefail

# Fix ownership of the named Docker volume mounted at /var/lib/ogma.
#
# Usage:
#   chmod +x fix-volume-perms.sh
#   ./fix-volume-perms.sh
#
# Optional:
#   ./fix-volume-perms.sh CONTAINER_NAME
#   ./fix-volume-perms.sh CONTAINER_NAME UID:GID
#
# The default container name matches the current Aspire Compose deployment.
CONTAINER="${1:-ogma3apphost-ogma3-1}"
UID_GID_OVERRIDE="${2:-}"
MOUNT_PATH="/var/lib/ogma"
HELPER_IMAGE="${HELPER_IMAGE:-alpine:latest}"

command -v docker >/dev/null 2>&1 || {
    echo "Error: docker was not found in PATH." >&2
    exit 1
}

docker inspect "$CONTAINER" >/dev/null 2>&1 || {
    echo "Error: container '$CONTAINER' was not found." >&2
    echo "Available containers:" >&2
    docker ps -a --format '  {{.Names}}' >&2
    exit 1
}

IMAGE="$(docker inspect --format '{{.Config.Image}}' "$CONTAINER")"

# Find the actual volume name from this container's mount configuration.
MOUNT_INFO="$(
    docker inspect --format \
        "{{range .Mounts}}{{if eq .Destination \"$MOUNT_PATH\"}}{{.Type}}|{{.Name}}{{end}}{{end}}" \
        "$CONTAINER"
)"

if [[ -z "$MOUNT_INFO" ]]; then
    echo "Error: '$MOUNT_PATH' is not mounted in '$CONTAINER'." >&2
    echo "Current mounts:" >&2
    docker inspect --format \
        '{{range .Mounts}}  {{.Type}} {{.Name}} -> {{.Destination}}{{println}}{{end}}' \
        "$CONTAINER" >&2
    exit 1
fi

IFS='|' read -r MOUNT_TYPE VOLUME_NAME <<< "$MOUNT_INFO"

if [[ "$MOUNT_TYPE" != "volume" || -z "$VOLUME_NAME" ]]; then
    echo "Error: '$MOUNT_PATH' is not backed by a named Docker volume." >&2
    echo "Mount type: $MOUNT_TYPE" >&2
    echo "This script intentionally does not chown bind mounts." >&2
    exit 1
fi

if [[ -n "$UID_GID_OVERRIDE" ]]; then
    if [[ ! "$UID_GID_OVERRIDE" =~ ^[0-9]+:[0-9]+$ ]]; then
        echo "Error: UID:GID must look like '1654:1654'." >&2
        exit 1
    fi
    APP_UID="${UID_GID_OVERRIDE%%:*}"
    APP_GID="${UID_GID_OVERRIDE##*:}"
else
    IMAGE_USER="$(docker image inspect --format '{{.Config.User}}' "$IMAGE")"

    if [[ -z "$IMAGE_USER" ]]; then
        # An empty image USER means the container runs as root by default.
        APP_UID=0
        APP_GID=0
    elif [[ "$IMAGE_USER" =~ ^[0-9]+(:[0-9]+)?$ ]]; then
        APP_UID="${IMAGE_USER%%:*}"
        if [[ "$IMAGE_USER" == *:* ]]; then
            APP_GID="${IMAGE_USER##*:}"
        else
            APP_GID="$APP_UID"
        fi
    else
        # Resolve a named image user (for example, "app") using id in the image.
        if ! APP_UID="$(docker run --rm --entrypoint id "$IMAGE" -u "$IMAGE_USER" 2>/dev/null)" ||
           ! APP_GID="$(docker run --rm --entrypoint id "$IMAGE" -g "$IMAGE_USER" 2>/dev/null)"; then
            echo "Error: couldn't resolve image user '$IMAGE_USER' to a numeric UID/GID." >&2
            echo "Run this script with an explicit override, for example:" >&2
            echo "  $0 '$CONTAINER' 1654:1654" >&2
            exit 1
        fi
    fi
fi

echo "Container: $CONTAINER"
echo "Image:     $IMAGE"
echo "Mount:     $VOLUME_NAME -> $MOUNT_PATH"
echo "Owner:     $APP_UID:$APP_GID"

# Stop the application while changing ownership to avoid concurrent file access.
WAS_RUNNING="$(docker inspect --format '{{.State.Running}}' "$CONTAINER")"
if [[ "$WAS_RUNNING" == "true" ]]; then
    echo "Stopping $CONTAINER..."
    docker stop "$CONTAINER" >/dev/null
fi

echo "Applying ownership (this may take a moment)..."
docker run --rm \
    --user 0:0 \
    -v "$VOLUME_NAME:/data" \
    "$HELPER_IMAGE" \
    chown -R "$APP_UID:$APP_GID" /data

echo "Volume ownership updated successfully."

if [[ "$WAS_RUNNING" == "true" ]]; then
    echo "Starting $CONTAINER..."
    docker start "$CONTAINER" >/dev/null
    echo "Container started."
else
    echo "Container was already stopped; it has been left stopped."
    echo "Start it with: docker start '$CONTAINER'"
fi
