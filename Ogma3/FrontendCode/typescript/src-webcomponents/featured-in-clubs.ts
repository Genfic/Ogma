import { LitElement, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { map } from "lit/directives/map.js";
import { when } from "lit/directives/when.js";
import { GetApiClubsStory as getFeaturingClubs } from "../generated/paths-public";
import type { GetClubsWithStoryResult } from "../generated/types-public";

@customElement("o-featured-in-clubs")
export class FeaturedInClubs extends LitElement {
	@property() accessor storyId: number;
	@state() private accessor visible: boolean;
	@state() private accessor clubs: GetClubsWithStoryResult[];

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");
		await this.fetch();
	}

	private async fetch() {
		const response = await getFeaturingClubs(this.storyId);
		if (response.ok) {
			this.clubs = response.data;
		}
	}

	private async open() {
		this.visible = true;
		await this.fetch();
	}

	private clubsView = () =>
		when(
			this.clubs.length > 0,
			() =>
				html`
					<div class="clubs">
						${map(
							this.clubs,
							(c) => html`
								<a href="/club/${c.id}/${c.name.toLowerCase().replace(" ", "-")}" target="_blank" class="club">
									<img src="${c.icon ?? "ph-250.png"}" alt="${c.name}" width="48" height="48" />
									<span>${c.name}</span>
									${map(c.folders.slice(0, 5), (f) => html`<span class="folder">${f}</span>`)}
									${when(c.folders.length > 5, () => html`<span class="overflow">+ ${c.folders.length - 5} more</span>`)}
								</a>
							`,
						)}
					</div>`,
			() => html`
				<div>This story hasn't been added to any clubs yet.</div>
			`,
		);

	render() {
		return html`
			<button class="club-wc-button" @click="${async () => await this.open()}">Featured in clubs</button>

			${when(
				this.visible,
				() => html`
					<div
						class="club-folder-selector my-modal"
						@click="${() => {
							this.visible = false;
						}}"
					>
						<div class="content" @click="${(e: Event) => e.stopPropagation()}">
							<div class="header">
								<span>Featured in</span>
							</div>

							${this.clubsView()}
						</div>
					</div>
				`,
			)}
		`;
	}

	createRenderRoot() {
		return this;
	}
}
