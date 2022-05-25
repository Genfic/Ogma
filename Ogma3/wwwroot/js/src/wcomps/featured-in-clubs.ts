import { html, LitElement } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { http } from "../helpers/http";
import { log } from "../helpers/logger";
import { Clubs_GetClubsWithStory as getFeaturingClubs } from "../generated/paths-public";

interface Club {
	id: number;
	name: string;
	icon: string;
}

@customElement('o-featured-in-clubs')
export class FeaturedInClubs extends LitElement {
	constructor() {
		super();
	}
	
	@property() storyId: number;
	@state() private visible: boolean;
	@state() private clubs: Club[];

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add('wc-loaded');
		await this.fetch();
	}

	private async fetch() {
		const response = await http.get<Club[]>(getFeaturingClubs(this.storyId));
		if (response.isSuccess) {
			this.clubs = response.getValue();
		} else {
			log.error(`Error fetching data: ${response.error}`);
		}
	}

	private async open() {
		this.visible = true;
		await this.fetch();
	}

	render() {
		return html`
            <a @click="${async () => await this.open()}">Featured in clubs</a>

            ${this.visible ? html`
                <div class="club-folder-selector my-modal" @click="${() => this.visible = false}">
                    <div class="content" @click="${e => e.stopPropagation()}">

                        <div class="header">
                            <span>Featured in</span>
                        </div>

                        ${this.clubs.length > 0 ? html`
                            <div class="clubs">
                                ${this.clubs.map(c => html`
                                    <a href="/club/${c.id}/${c.name.toLowerCase().replace(' ', '-')}"
                                       target="_blank"
                                       class="club"
                                       tabindex="0">
                                        <img src="${c.icon ?? 'ph-250.png'}" 
                                             alt="${c.name}" 
                                             width="24"
                                             height="24">
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