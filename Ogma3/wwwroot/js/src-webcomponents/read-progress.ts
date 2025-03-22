import { LitElement, css, html } from "lit";
import { customElement, state } from "lit/decorators.js";
import { styleMap } from "lit/directives/style-map.js";
import { clamp, normalize } from "../src-helpers/math-helpers";

@customElement("o-read-progress")
export class ReadProgress extends LitElement {
	@state() private progress = 0;
	@state() private ticking = false;
	@state() private read = false;

	// language=CSS
	static styles = css`
		:host {
			position: sticky;
			inset: auto 0 0;
		}

		.bar {
			position: relative;
			height: 3px;
			background-color: var(--accent);
			transition: width 50ms ease-out;
		}
	`;

	connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");

		document.addEventListener("scroll", () => {
			if (!this.ticking) {
				window.requestAnimationFrame(() => {
					this.handleScroll();
					this.ticking = false;
				});
				this.ticking = true;
			}
		});
		window.addEventListener("resize", () => this.handleScroll());
		this.handleScroll();
	}

	render() {
		return html`
			<div class="bar" style=${styleMap({ width: `${this.progress * 100}%` })}></div> `;
	}

	private handleScroll() {
		const elBottom = this.parentElement.getBoundingClientRect().bottom;
		const percent = elBottom - window.innerHeight;
		const containerHeight = this.parentElement.offsetTop + this.parentElement.offsetHeight;
		const maxHeight = Math.max(containerHeight - window.innerHeight, 0);

		this.progress = 1 - clamp(normalize(percent, 0, maxHeight));

		if (this.progress >= 1 && !this.read) {
			this.read = true;
			this.dispatchEvent(new CustomEvent("read"));
		}
	}
}
