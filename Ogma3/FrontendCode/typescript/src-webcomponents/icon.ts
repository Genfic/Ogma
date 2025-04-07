import { LitElement, html } from "lit";
import { customElement, property } from "lit/decorators.js";

@customElement("o-icon")
export class Icon extends LitElement {
	@property() accessor icon: string;

	render() {
		return html`
			<svg width="24" height="24" class="o-icon" part="icon" >
				<use href="/svg/spritesheet.svg#${this.icon}"></use>
			</svg>
		`;
	}
}
