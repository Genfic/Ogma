import {
    css,
    html,
    LitElement
} from 'https://cdn.skypack.dev/pin/lit@v2.0.2-YGLcP9t1W6IGdpcZ606E/mode=imports,min/optimized/lit.js';

export class CacheInfo extends LitElement {
    constructor() {
        super();
        this.sortBy = 'size';
        this.sortOrder = 'asc';
    }

    static get styles() {
        return css`
			.purge {
				cursor: pointer;
				padding: .5rem 1rem;
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

    static get properties() {
        return {
            cacheCount: {type: Array, attribute: false}
        };
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
				<button class="purge" @click="${() => this._purge()}">Purge</button>
			</div>
		`;
    }

    async _load() {
        const res = await fetch('/admin/api/cache');
        this.cacheCount = await res.json();
    }

    async _purge() {
        if (confirm("Are you sure?")) {
            const res = await fetch('/admin/api/cache', {
                method: 'DELETE'
            });
            await this._load();
        }
    }
}

window.customElements.define('cache-info', CacheInfo);