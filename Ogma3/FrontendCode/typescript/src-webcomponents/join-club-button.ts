import { PostApiClubjoin as joinClub, DeleteApiClubjoin as leaveClub } from "@g/paths-public";
import { log } from "@h/logger";
import { LitElement, html } from "lit";
import { customElement, property } from "lit/decorators.js";

@customElement("o-join")
export class JoinClubButton extends LitElement {
	@property() accessor clubId: number;
	@property() accessor csrf: string;
	@property() accessor isMember = false;

	connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");
	}

	render() {
		return html`
			<button
				class="button max ${this.isMember ? "leave" : "join"}"
				title="${this.isMember ? "Leave" : "Join"}"
				@click="${this.join}"
			>
				${this.isMember ? "Leave club" : "Join club"}
			</button>
		`;
	}

	private join = async () => {
		const send = this.isMember ? leaveClub : joinClub;

		const res = await send(
			{
				clubId: this.clubId,
			},
			{
				RequestVerificationToken: this.csrf,
			},
		);
		if (res.ok) {
			const data = res.data;
			this.isMember = data === true;
		} else {
			log.warn(res.statusText);
		}
	};

	createRenderRoot() {
		return this;
	}
}
