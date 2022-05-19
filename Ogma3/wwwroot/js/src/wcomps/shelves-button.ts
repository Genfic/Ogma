import { html, LitElement } from "lit";
import { http } from "../helpers/http";
import { log } from "../helpers/logger";
import { customElement, property, state } from "lit/decorators.js";

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

	@property() endpoint: string;
	@property() storyId: number;
	@property() csrf: string;
	@state() quickShelves: Shelf[] = [];
	@state() shelves: Shelf[] = [];
	@state() more: boolean = false;
	@state() page: number = 1;

	async connectedCallback() {
		super.connectedCallback();

		await this.#getQuickShelves();
		this.classList.add("wc-loaded");
	}

	render() {
		return html`
			${this.quickShelves?.map(
			(shelf) => html`
					<button
						class="shelf action-btn"
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
					</button>
				`
		)}

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
							${this.shelves?.map(
					(shelf) => html`
									<button
										class="action-btn"
										title="Add to ${shelf.name}"
										@click="${() =>
											this.#addOrRemove(shelf.id)}"
										style="box-shadow: ${shelf.doesContainBook
											? shelf.color + " inset 0 0 0 3px"
											: null}"
									>
										<i
											class="material-icons-outlined"
											style="color: ${shelf.color}"
										>
											${shelf.iconName ??
											"bookmark_border"}
										</i>
										<span>${shelf.name}</span>
									</button>
								`
				)}
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
		const res = await http.get<Shelf[]>(
			`${this.endpoint}/${this.storyId}/quick`
		);
		if (res.isSuccess) {
			this.quickShelves = res.getValue();
		} else {
			log.error(res.error);
		}
	}

	async #getShelves() {
		const res = await http.get<Shelf[]>(
			`${this.endpoint}/${this.storyId}?page=${this.page}`
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

		const res = await send(`${this.endpoint}/${id}/${this.storyId}`, null, {
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