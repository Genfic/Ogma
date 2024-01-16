import { LitElement, html } from "lit";
import { customElement, property } from "lit/decorators.js";
import { Users_FollowUser as followUser, Users_UnfollowUser as unfollowUser } from "../generated/paths-public";
import { log } from "../src-helpers/logger";

@customElement("o-follow")
export class FollowButton extends LitElement {
	constructor() {
		super();
	}

	@property() userName: string;
	@property() csrf: string;
	@property() isFollowed: boolean;

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");
	}

	render() {
		return html`
			<button
				class="button max ${this.isFollowed ? "leave" : "join"}"
				title="${this.isFollowed ? "Unfollow" : "Follow"}"
				@click="${this.#follow}"
			>
				${this.isFollowed ? "Following" : "Follow"}
			</button>
		`;
	}

	async #follow() {
		const send = this.isFollowed ? unfollowUser : followUser;

		const res = await send(
			{
				name: this.userName,
			},
			{
				RequestVerificationToken: this.csrf,
			},
		);
		if (res.ok) {
			this.isFollowed = await res.json();
		} else {
			log.warn(res.statusText);
		}
	}

	createRenderRoot() {
		return this;
	}
}
