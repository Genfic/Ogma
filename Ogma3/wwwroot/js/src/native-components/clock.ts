import {css, html, LitElement} from 'lit';
import {customElement, property} from 'lit/decorators.js';
import dayjs, {Dayjs} from 'dayjs';

@customElement('o-clock')
export class FollowButton extends LitElement {

	@property()
		date: Date;

	@property({attribute: false})
		time: Dayjs;

	constructor() {
		super();

		this.time = dayjs(this.date);

		setInterval(() => {
			this.time = this.time.add(1, 's');
		}, 1000);
	}


	static styles = css`
			time {
				font-family: "Courier New", Courier, monospace;
				letter-spacing: -2px;
				margin: auto 0;
			}
		`;

	connectedCallback() {
		super.connectedCallback();
		this.classList.add('wc-loaded');
	}

	protected render() {
		return html`
			<time class="timer" datetime="${this.time.format('yyyy-MM-dd HH:mm')}" title="Server time">
				${this.time.format('DD.MM.YYYY HH:mm:ss')}
			</time>
		`;
	}
}