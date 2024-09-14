import { css, html, LitElement } from "lit";
import { DeleteAdminApiCache, GetAdminApiCache } from "../../../generated/paths-internal";
import { customElement, property, state } from "lit/decorators.js";

@customElement("cache-info")
export class CacheInfo extends LitElement {
	constructor() {
		super();
	}
	
	@state()
	private cacheCount: number;
	
	@property()
	private csrf: string;

	static get styles() {
		return css`
			.purge {
				cursor: pointer;
				padding: 0.5rem 1rem;
				color: var(--foreground);
				background-color: var(--ele-1);
				border: 1px solid var(--foreground-50);
				transition: all 100ms ease-in-out;
			}
			.purge:hover {
				background-color: var(--red-25);
				border-color: var(--red);
				color: var(--red);
			}
		`;
	}

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");
		await this._load();
	}

	render() {
		return html`
			<div class="cache">
				<span class="count"><strong>${this.cacheCount}</strong> elements in the cache</span>
				<button
					class="purge"
					@click="${() => this._purge()}"
				>
					Purge
				</button>
			</div>
		`;
	}

	private async _load() {
		const res = await GetAdminApiCache();
		if (!res.ok) return;
		this.cacheCount = await res.json();
	}

	private async _purge() {
		if (confirm("Are you sure?")) {
			const res = await DeleteAdminApiCache({ RequestVerificationToken: this.csrf });
			if (!res.ok) return;
			await this._load();
		}
	}
}
