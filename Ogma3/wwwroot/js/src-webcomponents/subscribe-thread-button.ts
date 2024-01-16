import { LitElement, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import {
	Subscriptions_IsSubscribedToThread as isSubscribed,
	Subscriptions_SubscribeThread as subscribe,
	Subscriptions_UnsubscribeThread as unsubscribe,
} from "../generated/paths-public";
import { log } from "../src-helpers/logger";

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

		const res = await isSubscribed(this.threadId);
		if (res.ok) {
			this.subscribed = await res.json();
		} else {
			log.error(res.statusText);
		}
	}

	render() {
		return html`
			<button
				class="action-btn ${this.subscribed ? "active" : ""}"
				@click="${this.#subscribe}"
				title="${this.subscribed ? "Unsubscribe" : "Subscribe"}"
			>
				<i class="material-icons-outlined">${this.subscribed ? "notifications_active" : "notifications"}</i>&nbsp;
				${this.subscribed ? "Subscribed!" : "Subscribe"}
			</button>
		`;
	}

	async #subscribe() {
		const send = this.subscribed ? unsubscribe : subscribe;

		const res = await send({
			threadId: this.threadId,
		});
		if (res.ok) {
			this.subscribed = await res.json();
		} else {
			log.error(res.statusText);
		}
	}

	createRenderRoot() {
		return this;
	}
}
