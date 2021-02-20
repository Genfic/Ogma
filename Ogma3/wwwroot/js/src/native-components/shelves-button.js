import {LitElement, html} from 'https://cdn.skypack.dev/pin/lit-element@v2.4.0-wL9urDabdrJ7grkk3BAP/min/lit-element.js';

export class ShelvesButton extends LitElement {
    static get properties() {
        return {
            endpoint: { type: String },
            storyId: { type: Number },
            csrf: { type: String, attribute: false },
            shelves: { type: Array, attribute: false },
            more: { type: Array, attribute: false }
        }
    }
    
    constructor() {
        super();
        this.csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
    }
    
    connectedCallback() {
        super.connectedCallback();
        this._getShelves();
    }

    render() {
        return html`
            ${this.shelves?.filter(e => e.isQuick).slice(0, 5).map(shelf => html`
                <button
                    class="shelf action-btn"
                    title="'Add to ${shelf.name}"
                    @click="${() => this._add(shelf.id)}"
                    style="box-shadow: ${shelf.doesContainBook ? shelf.color + ' inset 0 0 0 3px' : null}">
                    <i class="material-icons-outlined" style="color: ${shelf.color}">
                        ${shelf.iconName ?? "bookmark_border"}
                    </i>
                </button>
            `)}

            <button title="All bookshelves" class="shelf action-btn" @click="${() => this.more = !this.more}">
                <i class="material-icons-outlined">more_horiz</i>
            </button>
            
            ${this.more ? html`
                <div class="more-shelves">
                    ${this.shelves?.filter(e => !e.isQuick).map(shelf => html`
                        <button
                            class="action-btn"
                            title="'Add to ${shelf.name}"
                            @click="${() => this._add(shelf.id)}"
                            style="box-shadow: ${shelf.doesContainBook ? shelf.color + ' inset 0 0 0 3px' : null}">
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
    
    _getShelves() {
        axios.get(`${this.endpoint}/user/${this.storyId}`)
            .then(res => {
                this.shelves = res.data || [];
            })
            .catch(console.error);
    }

    _add(id) {
        axios.post(`${this.endpoint}/add/${id}/${this.storyId}`,
            null,
            {
                headers: { "RequestVerificationToken" : this.csrf }
            })
            .then(_ => {
                this._getShelves();
            })
            .catch(console.error)
    }
    
    createRenderRoot() {
        return this;
    }
}

window.customElements.define('o-shelves', ShelvesButton);