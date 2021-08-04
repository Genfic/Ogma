import {
	LitElement,
	html
} from "https://cdn.skypack.dev/pin/lit-element@v2.4.0-wL9urDabdrJ7grkk3BAP/min/lit-element.js";

export class ShelvesButton extends LitElement {
	static get properties() {
		return {
			endpoint: { type: String },
			storyId: { type: Number },
			
			csrf: { type: String, attribute: false },
			quickShelves: { type: Array, attribute: false },
			shelves: { type: Array, attribute: false },
			more: { type: Array, attribute: false },
			page: { type: Number, attribute: false },
		};
	}

	constructor() {
		super();
		this.page = 1;
		this.csrf = document.querySelector("input[name=__RequestVerificationToken]").value;
	}

	async connectedCallback() {
		super.connectedCallback();
		console.log(this.endpoint);
		await this._getQuickShelves();
		this.classList.add("wc-loaded");
	}

	render() {
		return html`
          ${this.quickShelves?.map(shelf => html`
            <button
              class="shelf action-btn"
              title="Add to ${shelf.name}"
              @click="${() => this._addOrRemove(shelf.id)}"
              style="box-shadow: ${shelf.doesContainBook ? shelf.color + " inset 0 0 0 3px" : null}">
              <i class="material-icons-outlined" style="color: ${shelf.color}">
                ${shelf.iconName ?? "bookmark_border"}
              </i>
            </button>
          `)}

          <button title="All bookshelves" class="shelf action-btn" @click="${() => this._showMore()}">
            <i class="material-icons-outlined">more_horiz</i>
          </button>

          ${this.more ? html`
            <div class="more-shelves">
              ${this.shelves?.map(shelf => html`
                <button
                  class="action-btn"
                  title="Add to ${shelf.name}"
                  @click="${() => this._addOrRemove(shelf.id)}"
                  style="box-shadow: ${shelf.doesContainBook ? shelf.color + " inset 0 0 0 3px" : null}">
                  <i class="material-icons-outlined" style="color: ${shelf.color}">
                    ${shelf.iconName ?? "bookmark_border"}
                  </i>
                  <span>${shelf.name}</span>
                </button>
              `)}
            </div>
          ` : null}
		`;
	}
	
	async _showMore() {
		this.more = !this.more;
		if (this.more) await this._getShelves();
	}

	async _getQuickShelves() {
		const { data } = await axios.get(`${this.endpoint}/${this.storyId}/quick`);
		this.quickShelves = data;
	}
	
	async _getShelves() {
		const { data } = await axios.get(`${this.endpoint}/${this.storyId}?page=${this.page}`);
		this.shelves = data;
	}

	async _addOrRemove(id) {
		if ([...this.shelves ?? [], ...this.quickShelves ?? []].some(s => s.doesContainBook && s.id === id)) {
			await this._remove(id);
		} else {
			await this._add(id);
		}
	}
	
	async _add(id) {
		await axios.post(`${this.endpoint}/${id}/${this.storyId}`, null, 
			{ headers: { "RequestVerificationToken": this.csrf } }
		);
		await this._getQuickShelves();
		await this._getShelves();
	}

	async _remove(id) {
		await axios.delete(`${this.endpoint}/${id}/${this.storyId}`,
			{ headers: { "RequestVerificationToken": this.csrf } }
		);
		await this._getQuickShelves();
		await this._getShelves();
	}

	createRenderRoot() {
		return this;
	}
}

window.customElements.define("o-shelves", ShelvesButton);