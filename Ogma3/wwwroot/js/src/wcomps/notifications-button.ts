import { customElement, state } from "lit/decorators.js";
import { html, LitElement } from "lit";
import { log } from "../helpers/logger";
import { http } from "../helpers/http";
import { Notifications_CountUserNotifications as countNotifications } from "../generated/paths-public";

@customElement("o-notifications-button")
export class NotificationsButton extends LitElement {
	constructor() {
		super();
	}

	@state() notifications: number;

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");

		const res = await http.get<number>(countNotifications());
		if (res.isSuccess) {
			this.notifications = res.getValue();
		} else {
			log.error(res.error);
		}
	}

	#count = () =>
		this.notifications <= 99
			? this.notifications.clamp(0, 99).toString()
			: "99+";

	#title = () =>
		this.notifications > 0
			? `${this.notifications} notifications`
			: "Notifications";

	render() {
		return html`
			<a
				class="nav-link light notifications-btn"
				href="/notifications"
				title="${this.#title()}"
			>
				<i class="material-icons-outlined">notifications</i>
				${this.notifications > 0
					? html`<span>${this.#count()}</span>`
					: null}
			</a>
		`;
	}

	createRenderRoot() {
		return this;
	}
}
