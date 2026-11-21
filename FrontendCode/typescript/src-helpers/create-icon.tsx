import type { JSX } from "solid-js";

type IconProps = JSX.SvgSVGAttributes<SVGSVGElement> & { part?: string };

export const createIcon = (width: number, height: number, innerHTML: string) => {
	return function Icon(props: IconProps) {
		return (
			<svg
				viewBox={`0 0 ${width} ${height}`}
				width="1.4rem"
				height="1.4rem"
				part="icon"
				{...props}
				innerHTML={innerHTML}
			/>
		);
	};
};
