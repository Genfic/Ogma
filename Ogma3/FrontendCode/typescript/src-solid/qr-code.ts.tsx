import { parseDom } from "@h/dom";
import { customElement } from "solid-element";
import { createSignal, onMount } from "solid-js";
import { type QrCodeGenerateSvgOptions, renderSVG } from "uqr";

const opts: QrCodeGenerateSvgOptions = {
	pixelSize: 4,
	ecc: "M",
};

customElement(
	"qr-code",
	{
		width: Number,
		height: Number,
	},
	(props) => {
		const [svgs, setSvgs] = createSignal<string[]>([]);

		onMount(() => {
			const newSvgs: string[] = [];
			for (const child of props.element.children) {
				let svg = renderSVG(child.textContent, opts);

				if (props.width || props.height) {
					const svgDom = parseDom(svg);
					svgDom.setAttribute("width", (props.width ?? props.height).toString());
					svgDom.setAttribute("height", (props.height ?? props.width).toString());
					svg = svgDom.outerHTML;
				}

				newSvgs.push(svg);
			}
			props.element.innerHTML = "";
			setSvgs(newSvgs);
		});

		return () => (
			<>
				{svgs().map((svg) => (
					<div innerHTML={svg} />
				))}
			</>
		);
	},
);
