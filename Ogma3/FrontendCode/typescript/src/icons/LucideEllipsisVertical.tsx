import type { JSX } from "solid-js";

export function LucideEllipsisVertical(props: JSX.IntrinsicElements["svg"]) {
	return (
		<svg xmlns="http://www.w3.org/2000/svg" width="24px" height="24px" viewBox="0 0 24 24" {...props}>
			<title>lucide ellipsis vertical</title>
			<g fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2">
				<circle cx="12" cy="12" r="1" />
				<circle cx="12" cy="5" r="1" />
				<circle cx="12" cy="19" r="1" />
			</g>
		</svg>
	);
}
