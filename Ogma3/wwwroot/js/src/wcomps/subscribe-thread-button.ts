import { html, LitElement } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { http } from "../helpers/http";
import { log } from "../helpers/logger";

@customElement("o-subscribe")
export class SubscribeThreadButton extends LitElement {
	constructor() {
		super();
	}

	@property() endpoint: string;
	@property() threadId: number;
	@property() csrf: string;
	@state() subscribed: boolean;

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");

		const res = await http.get<boolean>(
			`${this.endpoint}/thread?threadId=${this.threadId}`
		);
		this.subscribed = res.isSuccess && res.getValue();

		res.isFailure && log.error(res.error);
	}

	render() {
		return html`
			<button
				class="action-btn ${this.subscribed ? "active" : ""}"
				@click="${this.#vote}"
				title="${this.subscribed ? "Unsubscribe" : "Subscribe"}"
			>
				<i class="material-icons-outlined"
					>${this.subscribed
						? "notifications_active"
						: "notifications"}</i
				>&nbsp; ${this.subscribed ? "Subscribed!" : "Subscribe"}
			</button>
		`;
	}

	async #vote() {
		const send = this.subscribed ? http.delete : http.post;

		const res = await send<boolean>(`${this.endpoint}/thread`, {
			threadId: this.threadId,
		});
		this.subscribed = res.isSuccess && res.getValue();

		res.isFailure && log.error(res.error);
	}

	createRenderRoot() {
		return this;
	}
}