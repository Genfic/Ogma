import { LitElement, css, html } from "lit";
import { customElement, property } from "lit/decorators.js";

@customElement("o-modal")
export class Modal extends LitElement {
	@property({ attribute: false }) accessor visible: boolean;

	connectedCallback() {
		super.connectedCallback();
	}

	public show() {
		this.visible = true;
	}

	public hide() {
		this.visible = false;
	}

	public toggle() {
		this.visible = !this.visible;
	}

	render() {
		return this.visible
			? html`
					<div class="my-modal" @click="${this.hide}">
						<div class="content" @click="${(e: Event) => e.stopPropagation()}">
							<slot></slot>
						</div>
					</div>
				`
			: null;
	}

	static style = css`
		.my-modal {
			position: fixed;
			display: flex;
			align-items: center;
			justify-content: center;
			inset: 0;
			width: 100%;
			height: 100%;
			z-index: 999;
			background: var(--foreground-50);
		}

		.content {
			min-width: 20rem;
			width: min(40rem, 100vw);
			max-height: 30rem;
			background: var(--background);
			padding: 1rem;
			height: 100%;
			overflow-y: auto;
			border: 5px solid var(--background);
		}
	`;
}
