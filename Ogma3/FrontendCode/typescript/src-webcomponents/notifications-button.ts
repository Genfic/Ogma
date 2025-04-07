import { LitElement, html, nothing } from "lit";
import { customElement, state } from "lit/decorators.js";
import { GetApiNotificationsCount as countNotifications } from "../generated/paths-public";
import { log } from "../src-helpers/logger";

@customElement("o-notifications-button")
export class NotificationsButton extends LitElement {
	@state() accessor notifications: number;

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");

		const res = await countNotifications();
		if (res.ok) {
			this.notifications = res.data;
		} else {
			log.error(res.statusText);
		}
	}

	private count = () => (this.notifications <= 99 ? this.notifications.toString() : "99+");

	private linkTitle = () => (this.notifications > 0 ? `${this.notifications} notifications` : "Notifications");

	render() {
		return html`
			<a class="nav-link light notifications-btn" href="/notifications" title="${this.linkTitle()}">
				<o-icon class="material-icons-outlined" icon="lucide:bell"></o-icon>
				${(this.notifications ?? -1) > 0 ? html`<span>${this.count()}</span>` : nothing}
			</a>
		`;
	}

	createRenderRoot() {
		return this;
	}
}
