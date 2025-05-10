import { parseDom } from "@h/dom";
import { type ComponentType, customElement } from "solid-element";
import { createSignal, onMount } from "solid-js";
import { type QrCodeGenerateSvgOptions, renderSVG } from "uqr";

type WidthOrHeight =
	| { width: number; height?: number }
	| { width?: number; height: number }
	| { width: number; height: number };

const QrCode: ComponentType<WidthOrHeight & { data: string } & QrCodeGenerateSvgOptions> = (props) => {
	const [svg, setSvg] = createSignal<string>("");

	const width = props.width ?? props.height;
	const height = props.height ?? props.width;

	onMount(() => {
		let svg = renderSVG(props.data, props);

		if (props.width || props.height) {
			const svgDom = parseDom(svg);
			svgDom.setAttribute("width", width.toString());
			svgDom.setAttribute("height", height.toString());
			svg = svgDom.outerHTML;
		}

		setSvg(svg);
	});

	return <div style={{ width: `${width}px`, height: `${height}px`, padding: "5px" }} innerHTML={svg()} />;
};

customElement(
	"qr-code",
	{
		width: null,
		height: null,
		data: "",
		ecc: "M",
		pixelSize: 4,
	},
	QrCode,
);
