# Rate limit

Rate limiting is partitioned by user ID first, then by IP address, then falls back to `0`.
Should a user who's not logged-in experience unusual throttling, it could be because they're
on a shared network (dorm, office, etc.) In that case, if they log in, they will be throttled
separately.

If some shady connection comes in (no user ID, no IP address), it just gets throttled with the
`0` bucket, shared by all such connections.
