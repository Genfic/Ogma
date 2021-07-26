import {LitElement, html} from 'https://cdn.skypack.dev/pin/lit-element@v2.4.0-wL9urDabdrJ7grkk3BAP/min/lit-element.js';

export class VoteButton extends LitElement {
	static get properties() {
		return {
			endpoint: { type: String },
			storyId: { type: Number },
			voted: { type: Boolean, attribute: false },
			score: { type: Number, attribute: false },
			csrf: { type: String, attribute: false }
		};
	}
    
	constructor() {
		super();
		this.csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
	}
    
	async connectedCallback() {
		super.connectedCallback();
		this.classList.add('wc-loaded');

		const { data } = await axios.get(`${this.endpoint}/${this.storyId}`);

		this.score = data.count;
		this.voted = data.didVote;
	}

	render() {
		return html`
            <button class="votes action-btn large ${this.voted ? 'active' : ''}" @click="${this._vote}" title="Give it a star!">
                <i class="material-icons-outlined">${this.voted ? 'star' : 'star_border'}</i>
                <span class="count">${this.score ?? 0}</span>
            </button>
        `;
	}
    
	async _vote() {
		const body = { storyId: this.storyId };
		const options = { headers: { "RequestVerificationToken": this.csrf } };

		const res = async () => this.voted 
			? await axios.delete(this.endpoint, {
				...options, data: body
			}) 
			: await axios.post(this.endpoint, body, options);
		
		const { data } = await res();
		
		this.score = data.count;
		this.voted = data.didVote;
	}

	createRenderRoot() {
		return this;
	}
}

window.customElements.define('o-vote', VoteButton);
