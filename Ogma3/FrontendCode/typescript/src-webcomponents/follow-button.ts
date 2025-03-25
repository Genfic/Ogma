import { LitElement, html } from "lit";
import { customElement, property } from "lit/decorators.js";
import { PostApiUsersFollow as followUser, DeleteApiUsersFollow as unfollowUser } from "../generated/paths-public";
import { log } from "../src-helpers/logger";

@customElement("o-follow")
export class FollowButton extends LitElement {
	@property() accessor userName: string;
	@property() accessor csrf: string;
	@property() accessor isFollowed: boolean;

	connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");
	}

	render() {
		return html`
			<button
				class="button max ${this.isFollowed ? "leave" : "join"}"
				title="${this.isFollowed ? "Unfollow" : "Follow"}"
				@click="${this.follow}"
			>
				${this.isFollowed ? "Following" : "Follow"}
			</button>
		`;
	}

	private async follow() {
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
			this.isFollowed = res.data;
		} else {
			log.warn(res.statusText);
		}
	}

	createRenderRoot() {
		return this;
	}
}
