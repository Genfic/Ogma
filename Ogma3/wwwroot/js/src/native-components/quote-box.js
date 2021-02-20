import {LitElement, html} from 'https://cdn.skypack.dev/pin/lit-element@v2.4.0-wL9urDabdrJ7grkk3BAP/min/lit-element.js';

export class QuoteBox extends LitElement {
    static get properties() {
        return {
            endpoint: { type: String },
            loading: { type: Boolean, attribute: false },
            body: { type: String, attribute: false },
            author: { type: String, attribute: false }
        }
    }

    constructor() {
        super();
    }

    connectedCallback() {
        super.connectedCallback();
        this._load();
    }

    render() {
        return html`
            <div id="quote" class="quote active-border">
                <div class="refresh" @click="${this._load}">
                    <i class="material-icons-outlined ${this.loading ? 'spin' : ''}">refresh</i>
                </div>
                <em class="body">${this.body}</em>
                <span class="author">${this.author}</span>
            </div>
        `;
    }

    _load() {
        axios.get(this.endpoint)
            .then(res => {
                this.body = res.data.body;
                this.author = res.data.author;
            })
            .catch(console.error);
    }

    createRenderRoot() {
        return this;
    }
}

window.customElements.define('o-quotebox', QuoteBox);