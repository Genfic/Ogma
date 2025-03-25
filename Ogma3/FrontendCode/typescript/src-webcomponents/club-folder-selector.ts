import { Task } from "@lit/task";
import { LitElement, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { classMap } from "lit/directives/class-map.js";
import { map } from "lit/directives/map.js";
import { when } from "lit/directives/when.js";
import {
	PostApiFoldersAddStory as addStoryToFolder,
	GetApiFolders as getFolders,
	GetApiClubsUser as getUserClubs,
} from "../generated/paths-public";
import type { GetFolderResult, GetJoinedClubsResponse } from "../generated/types-public";
import { log } from "../src-helpers/logger";

@customElement("o-club-folder-selector")
export class ClubFolderSelector extends LitElement {
	@property() accessor storyId: number;
	@property() accessor csrf: string;

	@state() private accessor clubs: GetJoinedClubsResponse[] = [];
	@state() private accessor selectedClub: GetJoinedClubsResponse | null = null;
	@state() private accessor status: { message: string; success: boolean } = {
		message: "",
		success: false,
	};
	@state() private accessor visible = false;

	@state() private accessor selectedFolder: GetFolderResult | null = null;

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");

		const response = await getUserClubs();
		if (response.ok) {
			this.clubs = response.data;
		} else {
			log.error(`Error fetching data: ${response.statusText}`);
		}
	}

	private _foldersTask = new Task(this, {
		task: async ([selectedClub], { signal }) => {
			if (!selectedClub) {
				throw new Error("Club not selected");
			}
			const res = await getFolders(selectedClub.id, null, { signal });
			if (!res.ok) {
				throw new Error(res.statusText);
			}
			return res.data;
		},
		args: () => [this.selectedClub],
	});

	private renderFolders = () =>
		this._foldersTask.render({
			pending: () => html`Loading folders...`,
			complete: (folders) =>
				map(
					folders,
					(folder) => html`
						 <button
							 class="folder ${classMap({
									locked: !folder.canAdd,
									active: this.selectedFolder?.id === folder.id,
								})}"
							 @click="${() => this.select(folder)}"
						 >
							 ${folder.name}
						 </button>
					 `,
				),
			error: (e) => html`Error: ${e}`,
		});

	private selectedClubView = () => {
		return html`
			<div class="header">
				<img src="${this.selectedClub.icon ?? "ph-250.png"}" alt="${this.selectedClub.name}" width="32" height="32" />
				<span>${this.selectedClub.name}</span>
			</div>

			<div class="msg ${this.status.success ? "success" : "error"}">${this.status.message}</div>

			<div class="folders">${this.renderFolders()}</div>

			<div class="buttons">
				<button class="active-border add" @click="${this.add}">Add</button>
				<button class="active-border cancel" @click="${() => this.setClub(null)}">Go back</button>
			</div>
		`;
	};

	private allClubsView = () => html`
		<div class="header">
			<span>Your clubs</span>
		</div>

		<div class="clubs">
			${map(
				this.clubs,
				(c) => html`
					<button class="club" @click="${() => this.setClub(c)}">
						<img src="${c.icon ?? "ph-250.png"}" alt="${c.name}" width="48" height="48" />
						<span>${c.name}</span>
					</button>
				`,
			)}
		</div>
	`;

	render() {
		return html`
			<button class="club-wc-button" @click="${() => this.setVisibility(true)}">Add to folder</button>
			${when(
				this.visible,
				() => html`
					<div class="club-folder-selector my-modal" @click="${() => this.setVisibility(false)}">
						<div class="content" @click="${(e: Event) => e.stopPropagation()}">
							${this.selectedClub !== null ? this.selectedClubView() : this.allClubsView()}
						</div>
					</div>
				`,
			)}
		`;
	}

	private select = (folder: GetFolderResult) => {
		log.info(`Selecting folder with ID ${folder.id}`);
		this.selectedFolder = folder;
	};

	private setClub = (club: GetJoinedClubsResponse | null) => {
		this.selectedClub = club;
	};

	private setVisibility = (visibility: boolean) => {
		this.visible = visibility;
	};

	private add = async () => {
		if (this.selectedFolder === null) {
			this.status = {
				message: "You must select a folder!",
				success: false,
			};
			return;
		}

		const response = await addStoryToFolder(
			{
				folderId: this.selectedFolder.id,
				storyId: this.storyId,
			},
			{
				RequestVerificationToken: this.csrf,
			},
		);

		if (response.ok) {
			this.status = {
				message: "Successfully added",
				success: true,
			};
		} else if (typeof response.data === "string") {
			this.status = {
				message: response.data.replaceAll('"', ""),
				success: false,
			};
		}
	};

	createRenderRoot() {
		return this;
	}
}
