import type { JSX } from "solid-js";
import "solid-js/jsx-runtime";

type IconProps = JSX.SvgSVGAttributes<SVGSVGElement> & { part?: string };

// noinspection JSUnusedGlobalSymbols It's a template loaded in icon-plugin.ts as raw text
export function createIcon(width: number, height: number, innerHTML: string) {
	return function Icon(props: IconProps) {
		return (
			<svg
				viewBox={`0 0 ${width} ${height}`}
				width="1.5em"
				height="1.5em"
				part="icon"
				{...props}
				innerHTML={innerHTML}
			/>
		);
	};
}
