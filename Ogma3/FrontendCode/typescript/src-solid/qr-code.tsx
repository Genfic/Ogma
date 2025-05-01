import { parseDom } from "@h/dom";
import { type ComponentType, customElement, noShadowDOM } from "solid-element";
import { createSignal, onMount } from "solid-js";
import { type QrCodeGenerateSvgOptions, renderSVG } from "uqr";

type WidthOrHeight =
	| { width: number; height?: number }
	| { width?: number; height: number }
	| { width: number; height: number };

const QrCode: ComponentType<WidthOrHeight & { data: string } & QrCodeGenerateSvgOptions> = (props) => {
	noShadowDOM();

	const [svg, setSvg] = createSignal<string>("");

	onMount(() => {
		let svg = renderSVG(props.data, props);

		if (props.width || props.height) {
			const svgDom = parseDom(svg);
			svgDom.setAttribute("width", (props.width ?? props.height).toString());
			svgDom.setAttribute("height", (props.height ?? props.width).toString());
			svg = svgDom.outerHTML;
		}

		setSvg(svg);
	});

	return <div innerHTML={svg()} />;
};

customElement(
	"qr-code",
	{
		width: 100,
		height: 100,
		data: "",
		ecc: "M",
		pixelSize: 4,
	},
	QrCode,
);
