import { LitElement, html } from "lit";
import { customElement, property } from "lit/decorators.js";
import { PostApiUsersBlock as blockUser, DeleteApiUsersBlock as unblockUser } from "../generated/paths-public";
import { log } from "../src-helpers/logger";

@customElement("o-block")
export class BlockUser extends LitElement {
	@property() userName: string;
	@property() csrf: string;
	@property() isBlocked: boolean;

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");
		this.addEventListener("click", this.block);
		this.addEventListener("keydown", (e: KeyboardEvent) => {
			if (e.key === " ") {
				this.block();
				e.preventDefault();
			}
		});
	}

	render() {
		return html`<span> ${this.isBlocked ? "Unblock" : "Block"} </span>`;
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
