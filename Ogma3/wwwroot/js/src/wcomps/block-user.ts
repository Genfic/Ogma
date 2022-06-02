import { customElement, property } from "lit/decorators.js";
import { html, LitElement } from "lit";
import { log } from "../helpers/logger";
import { http } from "../helpers/http";
import { Users_BlockUser as blockUser } from "../generated/paths-public";

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
		const send = this.isBlocked ? http.delete : http.post;

		const res = await send<boolean>(
			blockUser(),
			{
				name: this.userName,
			},
			{
				RequestVerificationToken: this.csrf,
			}
		);
		if (res.isSuccess) {
			this.isBlocked = res.getValue();
		} else {
			log.warn(res.error);
		}
	}

	createRenderRoot() {
		return this;
	}
}
