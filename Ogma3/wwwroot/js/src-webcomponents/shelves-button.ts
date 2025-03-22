import { LitElement, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import {
	PostApiShelfStories as addToShelf,
	GetApiShelfStoriesQuick as getQuickShelves,
	GetApiShelfStories as getShelves,
	DeleteApiShelfStories as removeFromShelf,
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
	@property() storyId: number;
	@property() csrf: string;
	@state() private quickShelves: Shelf[] = [];
	@state() private shelves: Shelf[] = [];
	@state() private more = false;
	@state() private page = 1;
	@state() private moreShelvesLoaded = false;

	async connectedCallback() {
		super.connectedCallback();

		await this.getQuickShelves();
		this.classList.add("wc-loaded");

		clickOutside(this, () => {
			this.more = false;
		});
	}

	private quickShelf = (shelf: Shelf) => html`
		<button
			class="shelf action-btn"
			title="Add to ${shelf.name}"
			@click="${() => this.addOrRemove(shelf.id)}"
			style="box-shadow: ${shelf.doesContainBook ? `${shelf.color} inset 0 0 0 3px` : null}"
		>
			<o-icon class="material-icons-outlined" style="color: ${shelf.color}" icon="${shelf.iconName}"></o-icon>
		</button>
	`;

	private shelf = (shelf: Shelf) => html`
		<button
			class="action-btn"
			title="Add to ${shelf.name}"
			@click="${() => this.addOrRemove(shelf.id)}"
			style="box-shadow: ${shelf.doesContainBook ? `${shelf.color} inset 0 0 0 3px` : null}"
		>
			<o-icon class="material-icons-outlined" style="color: ${shelf.color}" icon="${shelf.iconName}"></o-icon>
			<span>${shelf.name}</span>
		</button>
	`;

	render() {
		return html`
			${this.quickShelves?.map(this.quickShelf)}

			<button title="All bookshelves" class="shelf action-btn" @click="${() => this.showMore()}">
				<o-icon class="material-icons-outlined" icon="lucide:ellipsis-vertical"></o-icon>
			</button>

			${
				this.more
					? html`
				<div class="more-shelves">${this.shelves?.map(this.shelf)}</div> `
					: null
			}
		`;
	}

	private async showMore() {
		this.more = !this.more;
		if (this.more && !this.moreShelvesLoaded) {
			await this.getShelves();
		}
	}

	private async getQuickShelves() {
		const res = await getQuickShelves(this.storyId);
		if (res.ok) {
			this.quickShelves = res.data;
		} else {
			log.error(res.statusText);
		}
	}

	private async getShelves() {
		const res = await getShelves(this.storyId, this.page);
		if (res.ok) {
			this.shelves = res.data;
			this.moreShelvesLoaded = true;
		} else {
			log.error(res.statusText);
		}
	}

	private async addOrRemove(id: number) {
		const exists = [...this.shelves, ...this.quickShelves].some((s) => s.doesContainBook && s.id === id);
		const send = exists ? removeFromShelf : addToShelf;

		const res = await send(
			{
				storyId: this.storyId,
				shelfId: id,
			},
			{
				RequestVerificationToken: this.csrf,
			},
		);
		if (res.ok) {
			const data = res.data;
			const shelf = [...this.shelves, ...this.quickShelves].find((s) => s.id === data.shelfId);
			shelf.doesContainBook = !exists;
			this.requestUpdate();
			// await this.getQuickShelves();
			// await this.getShelves();
		} else {
			log.error(res.statusText);
		}
	}

	createRenderRoot() {
		return this;
	}
}
