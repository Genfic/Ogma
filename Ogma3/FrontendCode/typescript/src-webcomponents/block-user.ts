import { PostApiUsersBlock as blockUser, DeleteApiUsersBlock as unblockUser } from "@g/paths-public";
import { log } from "@h/logger";
import { LitElement, html } from "lit";
import { customElement, property } from "lit/decorators.js";

@customElement("o-block")
export class BlockUser extends LitElement {
	@property() accessor userName: string;
	@property() accessor csrf: string;
	@property() accessor isBlocked: boolean;

	render() {
		return html`<button @click="${this.block}"> ${this.isBlocked ? "Unblock" : "Block"} </button>`;
	}

	private async block() {
		const send = this.isBlocked ? unblockUser : blockUser;

		const res = await send(
			{
				name: this.userName,
			},
			{
				RequestVerificationToken: this.csrf,
			},
		);

		if (res.ok) {
			this.isBlocked = res.data;
		} else {
			log.warn(res.statusText);
		}
	}

	createRenderRoot() {
		return this;
	}
}
