import { html, LitElement } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { log } from "../helpers/logger";
import { http } from "../helpers/http";

interface Quote {
	body: string;
	author: string;
}

@customElement("quote-box")
export class QuoteBox extends LitElement {
	constructor() {
		super();
	}

	@property() endpoint: string;
	@state() private loading: boolean;
	@state() private quote: Quote;

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
				${this.quote
					? html`
							<em class="body">${this.quote.body}</em>
							<span class="author">${this.quote.author}</span>
					  `
					: html` <span>Loading the quote...</span> `}
			</div>
		`;
	}

	#spinnerClass = () => (this.loading ? "spin" : "");

	async load() {
		const res = await http.get<Quote>(this.endpoint);

		if (res.isFailure) {
			log.error(res.error);
			this.quote = JSON.parse(window.localStorage.getItem("quote"));
		} else {
			this.quote = res.getValue();
			window.localStorage.setItem("quote", JSON.stringify(this.quote));
		}
	}

	createRenderRoot() {
		return this;
	}
}