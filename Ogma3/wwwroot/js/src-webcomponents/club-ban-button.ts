import { LitElement, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { Clubs_BanUser as banUser, Clubs_UnbanUser as unbanUser } from "../generated/paths-public";
import { log } from "../src-helpers/logger";

@customElement("o-club-ban")
export class ClubBanButton extends LitElement {
	@property() accessor clubId: number;
	@property() accessor userId: number;
	@property() accessor csrf: string;
	@property() accessor banned: boolean;
	@state() accessor isBanned: boolean;

	connectedCallback() {
		super.connectedCallback();
		this.isBanned = this.banned;
		this.classList.add("wc-loaded");
	}

	render() {
		return html`
			<button class="votes action-btn large ${this.isBanned ? "active" : ""}" @click="${this.banOrUnban}">
				${this.isBanned ? "Unban" : "Ban"}
			</button>
		`;
	}

	private async banOrUnban() {
		const data = {
			clubId: this.clubId,
			userId: this.userId,
		};

		const headers = {
			RequestVerificationToken: this.csrf,
		};

		log.log(`Cid: ${this.clubId} Uid: ${this.userId}`);
		log.log({
			clubId: this.clubId,
			userId: this.userId,
			foo: 420,
		});

		const result = this.isBanned
			? await unbanUser(data, headers)
			: await banUser({ ...data, ...{ reason: prompt("What's the reason?") } }, headers);

		if (result.ok) {
			this.isBanned = await result.json();
		} else {
			log.error(`Error fetching data: ${result.statusText}`);
		}
	}

	createRenderRoot() {
		return this;
	}
}
