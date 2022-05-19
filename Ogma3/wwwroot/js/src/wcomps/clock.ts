import { css, html, LitElement } from "lit";
import { customElement, property } from "lit/decorators.js";
import { add, format, parseISO } from "date-fns";

@customElement("o-clock")
export class Clock extends LitElement {
	constructor() {
		super();

		setInterval(() => {
			this.date = add(this.date, { seconds: 1 });
		}, 1000);
	}

	@property({ converter: (value, _) => parseISO(value) })
	date: Date;

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
			<time
				class="timer"
				datetime="${format(this.date, "yyyy-MM-dd HH:mm")}"
				title="Server time"
			>
				${format(this.date, "dd.MM.yyyy HH:mm:ss")}
			</time>
		`;
	}
}