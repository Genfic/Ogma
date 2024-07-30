import { css, html, LitElement } from "lit";
import { customElement, state } from "lit/decorators.js";
import { styleMap } from "lit/directives/style-map.js";

@customElement("o-read-progress")
export class ReadProgress extends LitElement {
	constructor() {
		super();
	}

	@state() private accessor progress: number = 0;
	@state() private accessor windowHeight: number;
	@state() private accessor containerHeight: number;
	@state() private accessor ticking: boolean = false;
	@state() private accessor read: boolean = false;

	// language=CSS
	static styles = css`
		:host {
			position: fixed;
			position: sticky;
			bottom: 0;
			left: 0;
			padding: 0;
			margin: 0;
			width: 100%;
		}

		.bar {
			position: relative;
			height: 3px;
			margin-top: auto;
			background-color: var(--accent);
			transition: width 50ms ease-out;
		}
	`;

	connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");

		this.windowHeight = window.innerHeight;
		this.containerHeight = this.parentElement.offsetTop + this.parentElement.offsetHeight;

		document.addEventListener("scroll", () => {
			if (!this.ticking) {
				window.requestAnimationFrame(() => {
					this.#handleScroll();
					this.ticking = false;
				});
				this.ticking = true;
			}
		});
	}

	render() {
		return html`
			<div
				class="bar"
				style=${styleMap({ width: `${this.progress * 100}%` })}
			></div>
		`;
	}

	#handleScroll() {
		if (this.read) return;

		const elBottom = this.parentElement.getBoundingClientRect().bottom;
		const percent = elBottom - this.windowHeight;
		const maxHeight = Math.max(this.containerHeight - this.windowHeight, 0);
		this.progress = 1 - percent.normalize(0, maxHeight).clamp();
		if (this.progress >= 1) {
			this.read = true;
			console.log("Chapter read!");
			this.dispatchEvent(new CustomEvent("read"));
		}
	}
}
