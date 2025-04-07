import { LitElement, html, nothing } from "lit";
import { customElement, state } from "lit/decorators.js";
import { when } from "lit/directives/when.js";
import { GetApiQuotesRandom as getQuote } from "../generated/paths-public";
import { log } from "../src-helpers/logger";
import { classMap } from "lit/directives/class-map.js";

interface Quote {
	body: string;
	author: string;
}

@customElement("quote-box")
export class QuoteBox extends LitElement {
	@state() accessor _loading: boolean;
	@state() accessor _quote: Quote;
	@state() accessor _canReload = true;

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");
		await this.load();
	}

	render() {
		return html`
			<div id="quote" class="quote active-border">
				<div class="refresh" @click="${this.load}">
					<o-icon icon="${this.spinnerIcon()}" class="material-icons-outlined ${classMap({ spin: this._loading })}"></o-icon>
				</div>
				${when(
					this._quote,
					() => html`
						<em class="body">${this._quote.body}</em>
						<span class="author">${this._quote.author}</span>
					`,
					() => html` <span>Loading the quote...</span> `,
				)}
			</div>
		`;
	}
	private spinnerIcon = (): string => (this._canReload ? "lucide:refresh-cw" : "lucide:clock");

	async load() {
		if (!this._canReload) {
			return;
		}

		const response = await getQuote();
		if (response.ok) {
			this._quote = response.data;
			window.localStorage.setItem("quote", JSON.stringify(this._quote));
		} else if (response.status === 429) {
			this._quote = JSON.parse(window.localStorage.getItem("quote"));
		} else {
			log.error(response.statusText);
		}

		this._canReload = false;
		setTimeout(
			() => {
				this._canReload = true;
			},
			Number.parseInt(response.headers.get("retry-after")) * 1000,
		);
	}

	createRenderRoot() {
		return this;
	}
}
