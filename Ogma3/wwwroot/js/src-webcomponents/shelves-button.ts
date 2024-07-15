import { html, LitElement } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import {
	ShelfStories_AddToShelf as addToShelf,
	ShelfStories_GetUserQuickShelves as getQuickShelves,
	ShelfStories_GetUserShelvesPaginated as getShelves,
	ShelfStories_RemoveFromShelf as removeFromShelf,
} from "../generated/paths-public";
import { clickOutside } from "../src-helpers/click-outside";
import { log } from "../src-helpers/logger";

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

	@property() accessor storyId: number;
	@property() accessor csrf: string;
	@state() private accessor quickShelves: Shelf[] = [];
	@state() private accessor shelves: Shelf[] = [];
	@state() private accessor more: boolean = false;
	@state() private accessor page: number = 1;

	async connectedCallback() {
		super.connectedCallback();

		await this.#getQuickShelves();
		this.classList.add("wc-loaded");

		clickOutside(this, () => (this.more = false));
	}

	#quickShelf = (shelf: Shelf) => html`
		<button
			class="shelf action-btn"
			title="Add to ${shelf.name}"
			@click="${() => this.#addOrRemove(shelf.id)}"
			style="box-shadow: ${shelf.doesContainBook ? `${shelf.color} inset 0 0 0 3px` : null}"
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
				style="box-shadow: ${shelf.doesContainBook ? `${shelf.color} inset 0 0 0 3px` : null}"
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

			${
				this.more
					? html`
						<div class="more-shelves">
							${this.shelves?.map(this.#shelf)}
						</div>
				  `
					: null
			}
		`;
	}

	async #showMore() {
		this.more = !this.more;
		if (this.more) await this.#getShelves();
	}

	async #getQuickShelves() {
		const res = await getQuickShelves(this.storyId);
		if (res.ok) {
			this.quickShelves = await res.json();
		} else {
			log.error(res.statusText);
		}
	}

	async #getShelves() {
		const res = await getShelves(this.storyId, this.page);
		if (res.ok) {
			this.shelves = await res.json();
		} else {
			log.error(res.statusText);
		}
	}

	async #addOrRemove(id: number) {
		const exists = [...this.shelves, ...this.quickShelves].some((s) => s.doesContainBook && s.id === id);
		const send = exists ? removeFromShelf : addToShelf;

		const res = await send(id, this.storyId, {
			RequestVerificationToken: this.csrf,
		});
		if (res.ok) {
			await this.#getQuickShelves();
			await this.#getShelves();
		} else {
			log.error(res.statusText);
		}
	}

	createRenderRoot() {
		return this;
	}
}
