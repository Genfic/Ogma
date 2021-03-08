import {LitElement, html} from 'https://cdn.skypack.dev/pin/lit-element@v2.4.0-wL9urDabdrJ7grkk3BAP/min/lit-element.js';

export class NotificationsButton extends LitElement {
    static get properties() {
        return {
            endpoint: { type: String },
            href: { type: String },
            count: { type: Number, attribute: false }
        }
    }

    constructor() {
        super();
    }

    connectedCallback() {
        super.connectedCallback();
        this.load();
        this.classList.add('wc-loaded');
    }
    
    load() {
        axios.get(this.endpoint)
            .then(data => {
                this.count = data.data;
            })
            .catch(console.error)
    }

    render() {
        return html`
            <a class="nav-link light" href="${this.href}" title="${this.count ?? 0} notifications">
                <i class="material-icons-outlined wc-loaded">notifications</i>
                ${(this.count ?? 0) > 0 
                    ? html`<span class="badge">${this.count}</span>` 
                    : null 
                }
            </a>
        `;
    }

    createRenderRoot() {
        return this;
    }
}

window.customElements.define('o-notifications', NotificationsButton);