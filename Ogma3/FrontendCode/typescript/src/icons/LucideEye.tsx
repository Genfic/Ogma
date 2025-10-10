import type { JSX } from "solid-js";

export function LucideEye(props: JSX.IntrinsicElements["svg"]) {
	return (
		<svg xmlns="http://www.w3.org/2000/svg" width="24px" height="24px" viewBox="0 0 24 24" {...props}>
			<title>lucide eye</title>
			<g fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2">
				<path d="M2.062 12.348a1 1 0 0 1 0-.696a10.75 10.75 0 0 1 19.876 0a1 1 0 0 1 0 .696a10.75 10.75 0 0 1-19.876 0" />
				<circle cx="12" cy="12" r="3" />
			</g>
		</svg>
	);
}
