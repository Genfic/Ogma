import { customElement, property, state } from "lit/decorators.js";
import { html, LitElement } from "lit";
import { log } from "../helpers/logger";
import { http } from "../helpers/http";
import {
	Votes_GetVotes as getVotes,
	Votes_PostVote as postVote,
} from "../generated/paths-public";

interface VoteResponse {
	didVote: boolean;
	count: number;
}

@customElement("o-vote")
export class VoteButton extends LitElement {
	constructor() {
		super();
	}

	@property() storyId: number;
	@property() csrf: string;
	@state() private voted: boolean;
	@state() private score: number;

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");

		const result = await http.get<VoteResponse>(getVotes(this.storyId));
		if (result.isSuccess) {
			const data = result.getValue();
			this.score = data.count;
			this.voted = data.didVote;
		} else {
			log.error(`Error fetching data: ${result.error}`);
		}
	}

	render() {
		return html`
			<button
				class="votes action-btn large ${this.voted ? "active" : ""}"
				@click="${this.vote}"
				title="Give it a star!"
			>
				<i class="material-icons-outlined"
					>${this.voted ? "star" : "star_border"}</i
				>
				<span class="count">${this.score ?? 0}</span>
			</button>
		`;
	}

	private async vote() {
		const send = this.voted ? http.delete : http.post;
		const result = await send<VoteResponse>(
			postVote(),
			{ storyId: this.storyId },
			{
				RequestVerificationToken: this.csrf,
			}
		);

		if (result.isSuccess) {
			const data = result.getValue();
			this.score = data.count;
			this.voted = data.didVote;
		} else {
			log.error(`Error fetching data: ${result.error}`);
		}
	}

	createRenderRoot() {
		return this;
	}
}
