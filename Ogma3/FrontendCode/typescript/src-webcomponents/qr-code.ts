import { LitElement } from "lit";
import { customElement, property } from "lit/decorators.js";
import { unsafeSVG } from "lit/directives/unsafe-svg.js";
import { type QrCodeGenerateSvgOptions, renderSVG } from "uqr";
import { parseDom } from "../src-helpers/dom";

const opts: QrCodeGenerateSvgOptions = {
	pixelSize: 4,
	ecc: "M",
};

@customElement("qr-code")
export class QrCode extends LitElement {
	@property({ attribute: true }) accessor width: number | undefined;
	@property({ attribute: true }) accessor height: number | undefined;

	render() {
		const svgs: string[] = [];
		for (const child of this.children) {
			let svg = renderSVG(child.textContent, opts);

			if (this.width || this.height) {
				const svgDom = parseDom(svg);
				svgDom.setAttribute("width", (this.width ?? this.height).toString());
				svgDom.setAttribute("height", (this.height ?? this.width).toString());
				svg = svgDom.outerHTML;
			}

			svgs.push(svg);
		}
		this.innerHTML = "";
		return svgs.map((e) => unsafeSVG(e));
	}
}
