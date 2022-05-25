import { customElement, property } from "lit/decorators.js";
import { html, LitElement } from "lit";
import { log } from "../helpers/logger";
import { http } from "../helpers/http";
import { Users_FollowUser as followUser } from "../generated/paths-public";

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
				class="button max join"
				title="${this.isFollowed ? "Unfollow" : "Follow"}"
				@click="${this.#follow}"
			>
				${this.isFollowed ? "Following" : "Follow"}
			</button>
		`;
	}

	async #follow() {
		const send = this.isFollowed ? http.delete : http.post;

		const res = await send<boolean>(followUser(), {
			name: this.userName,
		});
		if (res.isSuccess) {
			this.isFollowed = res.getValue();
		} else {
			log.warn(res.error);
		}
	}

	createRenderRoot() {
		return this;
	}
}
