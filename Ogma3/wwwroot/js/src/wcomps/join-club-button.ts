import { customElement, property } from "lit/decorators.js";
import { html, LitElement } from "lit";
import { log } from "../helpers/logger";
import { ClubJoin_JoinClub as joinClub, ClubJoin_LeaveClub as leaveClub } from "../../generated/paths-public";

@customElement("o-join")
export class JoinClubButton extends LitElement {
	constructor() {
		super();
	}

	@property() clubId: number;
	@property() csrf: string;
	@property() isMember: boolean = false;

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");
	}

	render() {
		return html`
			<button
				class="button max ${this.isMember ? 'leave' : 'join'}"
				title="${this.isMember ? "Leave" : "Join"}"
				@click="${this.join}"
			>
				${this.isMember ? "Leave club" : "Join club"}
			</button>
		`;
	}

	private join = async() => {
		const send = this.isMember ? leaveClub : joinClub;

		const res = await send( {
			clubId: this.clubId,
		},{
			RequestVerificationToken: this.csrf,
		});
		if (res.ok) {
			this.isMember = await res.json();
		} else {
			log.warn(res.statusText);
		}
	};

	createRenderRoot() {
		return this;
	}
}
