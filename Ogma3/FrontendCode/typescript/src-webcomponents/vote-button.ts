import { DeleteApiVotes as deleteVote, GetApiVotes as getVotes, PostApiVotes as postVote } from "@g/paths-public";
import { log } from "@h/logger";
import { LitElement, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";

@customElement("o-vote")
export class VoteButton extends LitElement {
	@property() accessor storyId: number;
	@property() accessor csrf: string;

	@state() private accessor voted: boolean;
	@state() private accessor score: number;

	async connectedCallback() {
		super.connectedCallback();

		const result = await getVotes(this.storyId);
		if (result.ok) {
			const { count, didVote } = result.data;
			this.score = count;
			this.voted = didVote;
		} else {
			log.error(`Error fetching data: ${result.statusText}`);
		}
	}

	render() {
		return html`
			<button class="votes action-btn large ${this.voted ? "active" : ""}" @click="${this.vote}" title="Give it a star!">
				<o-icon icon="${this.voted ? "ic:round-star" : "ic:round-star-border"}" class="material-icons-outlined" ></o-icon>
				<span class="count">${this.score ?? 0}</span>
			</button>
		`;
	}

	private async vote() {
		const send = this.voted ? deleteVote : postVote;

		const result = await send(
			{ storyId: this.storyId },
			{
				RequestVerificationToken: this.csrf,
			},
		);

		if (result.ok) {
			const { count, didVote } = result.data;
			this.score = count;
			this.voted = didVote;
		} else {
			log.error(`Error fetching data: ${result.statusText}`);
		}
	}

	createRenderRoot() {
		return this;
	}
}
