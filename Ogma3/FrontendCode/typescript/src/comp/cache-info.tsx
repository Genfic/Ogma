import { DeleteAdminApiCache, GetAdminApiCache } from "@g/paths-internal";
import { type ComponentType, customElement } from "solid-element";
import { createResource, Match, Switch } from "solid-js";
import css from "./cache-info.css";
import { Styled } from "./common/_styled";

const CacheInfo: ComponentType<{ csrf: string }> = (props) => {
	let primed = $signal(false);

	const [cacheCount, { refetch }] = createResource(async () => {
		const res = await GetAdminApiCache();

		if (!res.ok) {
			console.warn(res.error);
			return Number.NaN;
		}

		if (typeof res.data !== "number") {
			console.warn("Invalid cache count:", res.data);
			return Number.NaN;
		}

		return res.data;
	});

	let timer: ReturnType<typeof setTimeout>;
	const mouseLeft = () => {
		timer = setTimeout(() => (primed = false), 500);
	};
	const mouseEntered = () => {
		clearTimeout(timer);
	};

	const purge = async () => {
		if (confirm("Are you sure?")) {
			const res = await DeleteAdminApiCache({ RequestVerificationToken: props.csrf });
			if (res.ok) {
				await refetch();
			} else {
				console.error("Failed to purge cache:", res);
			}
		} else {
			primed = false;
		}
	};

	// The JSX template
	return (
		<div class="cache">
			<span class="count">
				<strong>{cacheCount()}</strong> elements in the cache
			</span>
			<Switch>
				<Match when={primed}>
					<button
						type="button"
						class="purge"
						onClick={purge}
						onmouseleave={mouseLeft}
						onmouseenter={mouseEntered}
					>
						Purge
					</button>
				</Match>
				<Match when={!primed}>
					<button type="button" class="prime" onClick={() => (primed = true)}>
						Prime
					</button>
				</Match>
			</Switch>
		</div>
	);
};

customElement("cache-info", { csrf: "" }, Styled(CacheInfo, css));
