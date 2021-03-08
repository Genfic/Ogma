import {LitElement, html} from 'https://cdn.skypack.dev/pin/lit-element@v2.4.0-wL9urDabdrJ7grkk3BAP/min/lit-element.js';

export class VoteButton extends LitElement {
    static get properties() {
        return {
            endpoint: { type: String },
            storyId: { type: Number },
            voted: { type: Boolean, attribute: false },
            score: { type: Number, attribute: false },
            csrf: { type: String, attribute: false }
        }
    }
    
    constructor() {
        super();
        this.csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
    }
    
    connectedCallback() {
        super.connectedCallback();
        this.classList.add('wc-loaded');

        axios.get(this.endpoint + '/' + this.storyId)
            .then(res => {
                this.score = res.data.count;
                this.voted = res.data.didVote;
            })
            .catch(console.error);
    }

    render() {
        return html`
            <button class="votes action-btn large ${this.voted ? 'active' : ''}" @click="${this._vote}" title="Give it a star!">
                <i class="material-icons-outlined">${this.voted ? 'star' : 'star_border'}</i>
                <span class="count">${this.score ?? 0}</span>
            </button>
        `;
    }
    
    _vote() {
        axios.post(this.endpoint, {
                storyId: this.storyId
            }, {
                headers: { "RequestVerificationToken" : this.csrf }
            })
            .then(res => {
                this.score = res.data.count;
                this.voted = res.data.didVote;
            })
            .catch(console.error)
    }

    createRenderRoot() {
        return this;
    }
}

window.customElements.define('o-vote', VoteButton);
