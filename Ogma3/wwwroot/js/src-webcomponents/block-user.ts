import { customElement, property } from "lit/decorators.js";
import { html, LitElement } from "lit";
import { log } from "../src-helpers/logger";
import { Users_BlockUser as blockUser, Users_UnblockUser as unblockUser } from "../generated/paths-public";

@customElement("o-block")
export class BlockUser extends LitElement {
	constructor() {
		super();
	}

	@property() userName: string;
	@property() csrf: string;
	@property() isBlocked: boolean;

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");
		this.addEventListener("click", this.#block);
		this.addEventListener("keydown", (e: KeyboardEvent) => {
			if (e.key === " ") {
				this.#block();
				e.preventDefault();
			}
		});
	}

	render() {
		return html`<span> ${this.isBlocked ? "Unblock" : "Block"} </span>`;
	}

	async #block() {
		const send = this.isBlocked ? unblockUser : blockUser;

		const res = await send({
			name: this.userName,
		},{
			RequestVerificationToken: this.csrf,
		});
		
		if (res.ok) {
			this.isBlocked = await res.json();
		} else {
			log.warn(res.statusText);
		}
	}

	createRenderRoot() {
		return this;
	}
}
