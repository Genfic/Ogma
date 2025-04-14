import { parseDom } from "@h/dom";
import { type ComponentType, customElement, noShadowDOM } from "solid-element";
import { For, createSignal, onMount } from "solid-js";
import { type QrCodeGenerateSvgOptions, renderSVG } from "uqr";

const opts: QrCodeGenerateSvgOptions = {
	pixelSize: 4,
	ecc: "M",
};

type WidthOrHeight =
	| { width: null | undefined; height: number }
	| { width: number; height: null | undefined }
	| { width: number; height: number };

const QrCode: ComponentType<WidthOrHeight> = (props, { element }) => {
	noShadowDOM();

	const [svgs, setSvgs] = createSignal<string[]>([]);

	onMount(() => {
		const newSvgs: string[] = [];
		for (const child of element.children) {
			let svg = renderSVG(child.textContent, opts);

			if (props.width || props.height) {
				const svgDom = parseDom(svg);
				svgDom.setAttribute("width", (props.width ?? props.height).toString());
				svgDom.setAttribute("height", (props.height ?? props.width).toString());
				svg = svgDom.outerHTML;
			}

			newSvgs.push(svg);
		}
		element.innerHTML = "";
		setSvgs(newSvgs);
	});

	return <For each={svgs()}>{(item) => <div innerHTML={item} />}</For>;
};

customElement(
	"qr-code",
	{
		width: 0,
		height: 0,
	},
	QrCode,
);
