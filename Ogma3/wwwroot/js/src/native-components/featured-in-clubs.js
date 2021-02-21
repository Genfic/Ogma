import {LitElement, html} from 'https://cdn.skypack.dev/pin/lit-element@v2.4.0-wL9urDabdrJ7grkk3BAP/min/lit-element.js';

export class FeaturedInClubs extends LitElement {
    static get properties() {
        return {
            endpoint: { type: String },
            storyId: { type: Number },
            cdn: { type: String },
            visible: { type: Boolean, attribute: false, },
            clubs: { type: Array, attribute: false} 
        }
    }

    constructor() {
        super();
    }

    connectedCallback() {
        super.connectedCallback();
        axios.get(`${this.endpoint}/story/${this.storyId}`)
            .then(res => this.clubs = res.data)
            .catch(console.error);
    }

    render() {
        return html`
            <a @click="${() => this.visible = !this.visible}">Featured in clubs</a>
            
            ${this.visible ? html`
                <div class="club-folder-selector my-modal" @click="${() => this.visible = false}" >
                    <div class="content" @click="${e => e.stopPropagation()}">

                        <div class="header">
                            <span>Featured in</span>
                        </div>
                        
                        ${this.clubs.length > 0 ? html`
                            <div class="clubs">
                                ${this.clubs.map(c => html`
                                    <a href="/club/${c.name.toLowerCase().replace(' ', '-')}-${c.id}"
                                       target="_blank"
                                       class="club"
                                       tabindex="0">
                                        <img src="${this.cdn}${c.icon ?? 'ph-250.png'}" alt="${c.name}" width="24" height="24">
                                        <span>${c.name}</span>
                                    </a>
                                `)}
                            </div>
                        ` : html`
                            <div>
                                This story hasn't been added to any clubs yet.
                            </div>
                        `}
                    </div>
                </div>
            ` : null}
        `;
    }

    createRenderRoot() {
        return this;
    }
}

window.customElements.define('o-featured-in-clubs', FeaturedInClubs);