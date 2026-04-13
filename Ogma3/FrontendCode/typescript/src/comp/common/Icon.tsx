import { omit } from "es-toolkit";
import type { JSX } from "solid-js";

export const Icon = (props: { name: string } & JSX.SvgSVGAttributes<SVGSVGElement>) => {
	const p = { width: 24, height: 24, viewBox: "0 0 24 24", "aria-hidden": true, ...omit(props, ["name"]) };
	return (
		<svg {...p}>
			<title>{props.name}</title>
			<use href={`/spritesheet.svg#${props.name}`} />
		</svg>
	);
};
