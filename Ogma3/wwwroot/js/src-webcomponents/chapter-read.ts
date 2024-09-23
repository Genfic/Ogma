import { html, LitElement } from "lit";
import { customElement, property } from "lit/decorators.js";

@customElement("o-read")
class ChapterRead extends LitElement {
	@property() accessor route: string;
	@property() accessor chapterId: number;

	connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");
	}

	render() {
		return html`
			<button
				class="read-status"
				aria-label="Chapter read status"
				data-id="@c.Id"
			>
				<i class="material-icons-outlined">visibility_off</i>
			</button>
		`;
	}
}
