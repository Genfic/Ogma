import { LitElement, html } from "lit";
import { customElement, property } from "lit/decorators.js";

@customElement("o-read")
class ChapterRead extends LitElement {
	@property() route: string;
	@property() chapterId: number;

	connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");
	}

	render() {
		return html`
			<button class="read-status" aria-label="Chapter read status" data-id="@c.Id">
				<o-icon icon="lucide:eye-off"></o-icon>
			</button>
		`;
	}
}
