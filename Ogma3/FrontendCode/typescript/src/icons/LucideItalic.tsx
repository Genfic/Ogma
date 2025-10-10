import type { JSX } from "solid-js";

export function LucideItalic(props: JSX.IntrinsicElements["svg"]) {
	return (
		<svg xmlns="http://www.w3.org/2000/svg" width="24px" height="24px" viewBox="0 0 24 24" {...props}>
			<title>lucide italic</title>
			<path
				fill="none"
				stroke="currentColor"
				stroke-linecap="round"
				stroke-linejoin="round"
				stroke-width="2"
				d="M19 4h-9m4 16H5M15 4L9 20"
			/>
		</svg>
	);
}
