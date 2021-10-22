import {html, LitElement} from "https://cdn.skypack.dev/pin/lit-element@v2.4.0-wL9urDabdrJ7grkk3BAP/min/lit-element.js";

export class QuoteBox extends LitElement {
	static get properties() {
		return {
			endpoint: { type: String },
			loading: { type: Boolean, attribute: false },
			body: { type: String, attribute: false },
			author: { type: String, attribute: false }
		};
	}

	constructor() {
		super();
	}

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");
		await this._load();
	}

	render() {
		return html`
          <div id="quote" class="quote active-border">
            <div class="refresh" @click="${this._load}">
              <i class="material-icons-outlined ${this.loading ? "spin" : ""}">refresh</i>
            </div>
            <em class="body">${this.body}</em>
            <span class="author">${this.author}</span>
          </div>
		`;
	}

	async _load() {
		try {
			const res = await axios.get(this.endpoint);
			({ body: this.body, author: this.author } = res.data);
			window.localStorage.setItem("quote", JSON.stringify(res.data));
		} catch (e) {
			if (e.response.status === 429) log.log("Too many requests, loading from cache");
			else log.error(e);
			({ body: this.body, author: this.author } = JSON.parse(window.localStorage.getItem("quote")));
		}
	}

	createRenderRoot() {
		return this;
	}
}

window.customElements.define("o-quotebox", QuoteBox);