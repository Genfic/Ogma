import { omit } from "es-toolkit";
import type { JSX } from "solid-js";

export type IconProps = { name: string } & JSX.SvgSVGAttributes<SVGSVGElement>;

export const Icon = (props: IconProps) => {
	const p = { width: 24, height: 24, viewBox: "0 0 24 24", "aria-hidden": true, ...omit(props, ["name"]) };
	return (
		<svg {...p}>
			<title>{props.name}</title>
			<use href={`/spritesheet.svg#${props.name}`} />
		</svg>
	);
};
