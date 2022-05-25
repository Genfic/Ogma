import { html, LitElement } from "lit";
import { http } from "../helpers/http";
import { log } from "../helpers/logger";
import { customElement, property, state } from "lit/decorators.js";
import {
	ShelfStories_AddToShelf as addToShelf,
	ShelfStories_GetUserQuickShelves as getQuickShelves,
	ShelfStories_GetUserShelvesPaginated as getShelves,
} from "../generated/paths-public";
import { clickOutside } from "../helpers/click-outside";

interface Shelf {
	id: number;
	name: string;
	doesContainBook: boolean;
	color: string;
	iconName: string;
}

@customElement("o-shelves")
export class ShelvesButton extends LitElement {
	constructor() {
		super();
	}

	@property() storyId: number;
	@property() csrf: string;
	@state() private quickShelves: Shelf[] = [];
	@state() private shelves: Shelf[] = [];
	@state() private more: boolean = false;
	@state() private page: number = 1;

	async connectedCallback() {
		super.connectedCallback();

		await this.#getQuickShelves();
		this.classList.add("wc-loaded");
		
		clickOutside(this, () => this.more = false);
	}

	#quickShelf = (shelf: Shelf) => html`
		<button
			class="shelf action-btn"
			title="Add to ${shelf.name}"
			@click="${() => this.#addOrRemove(shelf.id)}"
			style="box-shadow: ${shelf.doesContainBook
				? shelf.color + " inset 0 0 0 3px"
				: null}"
		>
			<i class="material-icons-outlined" style="color: ${shelf.color}">
				${shelf.iconName ?? "bookmark_border"}
			</i>
		</button>
	`;

	#shelf = (shelf: Shelf) =>
		html`
			<button
				class="action-btn"
				title="Add to ${shelf.name}"
				@click="${() => this.#addOrRemove(shelf.id)}"
				style="box-shadow: ${shelf.doesContainBook
					? shelf.color + " inset 0 0 0 3px"
					: null}"
			>
				<i
					class="material-icons-outlined"
					style="color: ${shelf.color}"
				>
					${shelf.iconName ?? "bookmark_border"}
				</i>
				<span>${shelf.name}</span>
			</button>
		`;

	render() {
		return html`
			${this.quickShelves?.map(this.#quickShelf)}

			<button
				title="All bookshelves"
				class="shelf action-btn"
				@click="${() => this.#showMore()}"
			>
				<i class="material-icons-outlined">more_horiz</i>
			</button>

			${this.more
				? html`
						<div class="more-shelves">
							${this.shelves?.map(this.#shelf)}
						</div>
				  `
				: null}
		`;
	}

	async #showMore() {
		this.more = !this.more;
		if (this.more) await this.#getShelves();
	}

	async #getQuickShelves() {
		const res = await http.get<Shelf[]>(getQuickShelves(this.storyId));
		if (res.isSuccess) {
			this.quickShelves = res.getValue();
		} else {
			log.error(res.error);
		}
	}

	async #getShelves() {
		const res = await http.get<Shelf[]>(
			getShelves(this.storyId, this.page)
		);
		if (res.isSuccess) {
			this.shelves = res.getValue();
		} else {
			log.error(res.error);
		}
	}

	async #addOrRemove(id) {
		const exists = [...this.shelves, ...this.quickShelves].some(
			(s) => s.doesContainBook && s.id === id
		);
		const send = exists ? http.delete : http.post;

		const res = await send(addToShelf(id, this.storyId), null, {
			RequestVerificationToken: this.csrf,
		});
		if (res.isSuccess) {
			await this.#getQuickShelves();
			await this.#getShelves();
		} else {
			log.error(res.error);
		}
	}

	createRenderRoot() {
		return this;
	}
}