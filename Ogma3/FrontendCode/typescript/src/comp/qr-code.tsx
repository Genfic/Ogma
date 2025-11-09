import { parseDom } from "@h/dom";
import { component } from "@h/web-components";
import type { ComponentType } from "solid-element";
import { onMount } from "solid-js";
import { type QrCodeGenerateSvgOptions, renderSVG } from "uqr";

type WidthOrHeight =
	| { width: number; height?: number }
	| { width?: number; height: number }
	| { width: number; height: number };

const QrCode: ComponentType<WidthOrHeight & { data: string } & QrCodeGenerateSvgOptions> = (props) => {
	let svg = $signal<string>("");

	const width = props.width ?? props.height ?? 0;
	const height = props.height ?? props.width ?? 0;

	onMount(() => {
		let newSvg = renderSVG(props.data, props);

		if (props.width || props.height) {
			const svgDom = parseDom(newSvg);
			svgDom.setAttribute("width", width.toString());
			svgDom.setAttribute("height", height.toString());
			newSvg = svgDom.outerHTML;
		}

		svg = newSvg;
	});

	return <div style={{ width: `${width}px`, height: `${height}px`, padding: "5px" }} innerHTML={svg} />;
};

component(
	"qr-code",
	{
		width: 100,
		height: undefined,
		data: "",
		ecc: "M",
		pixelSize: 4,
	},
	QrCode,
);
