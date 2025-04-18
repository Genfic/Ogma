import { addToDate } from "@h/date-helpers";
import { EU, iso8601 } from "@h/tinytime-templates";
import { LitElement, css, html } from "lit";
import { customElement, property } from "lit/decorators.js";

@customElement("o-clock")
export class Clock extends LitElement {
	constructor() {
		super();

		setInterval(() => {
			this.date = addToDate(this.date, { seconds: 1 });
		}, 1000);
	}

	@property({ converter: (value, _) => new Date(value) })
	accessor date: Date;

	// language=CSS
	static styles = css`
		time {
			font-family: "Courier New", Courier, monospace;
			letter-spacing: -2px;
			margin: auto 0;
		}
	`;

	connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");
	}

	render() {
		return html`
			<time class="timer" datetime="${iso8601.render(this.date)}" title="Server time">
				${EU.render(this.date)}
			</time>
		`;
	}
}
