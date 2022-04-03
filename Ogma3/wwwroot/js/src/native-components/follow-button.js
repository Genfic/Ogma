import {html, LitElement} from 'https://cdn.skypack.dev/pin/lit-element@v2.4.0-wL9urDabdrJ7grkk3BAP/min/lit-element.js';

export class FollowButton extends LitElement {
    constructor() {
        super();
        this.csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
    }

    static get properties() {
        return {
            endpoint: {type: String},
            userName: {type: String},
            isFollowed: {type: String},
            csrf: {type: String, attribute: false}
        };
    }

    connectedCallback() {
        super.connectedCallback();
        this.classList.add('wc-loaded');
    }

    render() {
        return html`
            <button class="button max join"
                    title="${this.isFollowed.toLowerCase() === 'true' ? 'Unfollow' : 'Follow'}"
                    @click="${this._follow}">
                ${this.isFollowed.toLowerCase() === 'true' ? 'Following' : 'Follow'}
            </button>
        `;
    }

    _follow() {
        axios.post(`${this.endpoint}/follow`,
            {name: this.userName},
            {
                headers: {'RequestVerificationToken': this.csrf}
            }
        )
            .then(res => this.isFollowed = res.data.toString())
            .catch(log.error);
    }

    createRenderRoot() {
        return this;
    }
}

window.customElements.define('o-follow', FollowButton);
