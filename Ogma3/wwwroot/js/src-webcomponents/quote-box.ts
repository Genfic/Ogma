import { html, LitElement } from "lit";
import { customElement, state } from "lit/decorators.js";
import { log } from "../src-helpers/logger";
import { Quotes_GetRandomQuote as getQuote } from "../generated/paths-public";

interface Quote {
	body: string;
	author: string;
}

@customElement("quote-box")
export class QuoteBox extends LitElement {
	constructor() {
		super();
	}

	@state() private _loading: boolean;
	@state() private _quote: Quote;

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");
		await this.load();
	}

	render() {
		return html`
			<div id="quote" class="quote active-border">
				<div class="refresh" @click="${this.load}">
					<i class="material-icons-outlined ${this.#spinnerClass()}">
						refresh
					</i>
				</div>
				${this._quote
					? html`
							<em class="body">${this._quote.body}</em>
							<span class="author">${this._quote.author}</span>
					  `
					: html` <span>Loading the quote...</span> `}
			</div>
		`;
	}

	#spinnerClass = () => this._loading ? "spin" : "";

	async load() {
		const response = await getQuote();
		if (response.ok) {
			this._quote = await response.json();
			window.localStorage.setItem("quote", JSON.stringify(this._quote));
		} else {
			log.error(response.statusText);
			this._quote = JSON.parse(window.localStorage.getItem("quote"));
		}		
	}

	createRenderRoot() {
		return this;
	}
}