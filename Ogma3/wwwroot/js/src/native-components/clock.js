import {
    css,
    html,
    LitElement
} from 'https://cdn.skypack.dev/pin/lit-element@v2.4.0-wL9urDabdrJ7grkk3BAP/min/lit-element.js';

export class FollowButton extends LitElement {
    constructor() {
        super();

        this.time = dayjs(this.date);

        setInterval(() => {
            this.time = this.time.add(1, 's');
        }, 1000);
    }

    static get properties() {
        return {
            date: {type: Date},
            time: {type: dayjs, attribute: false},
        };
    }

    static get styles() {
        return css`
			time {
				font-family: "Courier New", Courier, monospace;
				letter-spacing: -2px;
				margin: auto 0;
			}
		`;
    }

    connectedCallback() {
        super.connectedCallback();
        this.classList.add('wc-loaded');
    }

    render() {
        return html`
            <time class="timer" datetime="${this.time.format('yyyy-MM-dd HH:mm')}" title="Server time">
                ${this.time.format('DD.MM.YYYY HH:mm:ss')}
            </time>
        `;
    }
}

window.customElements.define('o-clock', FollowButton);