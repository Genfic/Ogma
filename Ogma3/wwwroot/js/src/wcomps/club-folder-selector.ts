import { customElement, property, state } from "lit/decorators.js";
import { html, LitElement } from "lit";
import { log } from "../helpers/logger";
import { http } from "../helpers/http";
import { createRef, ref } from "lit/directives/ref.js";
import { FolderTree } from "./folder-tree";

interface Club {
	id: number;
	name: string;
	icon: string;
}

@customElement("o-club-folder-selector")
export class ClubFolderSelector extends LitElement {
	constructor() {
		super();
	}

	@property() clubsRoute: string;
	@property() foldersRoute: string;
	@property() storyId: number;
	@property() csrf: string;

	@state() private clubs: Club[];
	@state() private folders: {}[];
	@state() private selectedClub: Club | null = null;
	@state() private status: { message: string; success: boolean } = {
		message: "",
		success: false,
	};
	@state() private visible: boolean = false;

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");

		const response = await http.get<Club[]>(`${this.clubsRoute}/user`);
		if (response.isSuccess) {
			this.clubs = response.getValue();
		} else {
			log.error(`Error fetching data: ${response.error}`);
		}
	}

	#selectedClubView = () => html`
		<div class="header">
			<img
				src="${this.selectedClub.icon ?? "ph-250.png"}"
				alt="${this.selectedClub.name}"
				width="32"
				height="32"
			/>
			<span>${this.selectedClub.name}</span>
		</div>

		<div class="msg ${this.status.success ? "success" : "error"}">
			${this.status.message}
		</div>

		<o-folder-tree
			${ref(this.#treeRef)}
			clubId="${this.selectedClub.id}"
			route="${this.foldersRoute}"
		>
		</o-folder-tree>

		<div class="buttons">
			<button class="active-border add" @click="${this.#add}">Add</button>
			<button
				class="active-border cancel"
				@click="${() => (this.selectedClub = null)}"
			>
				Go back
			</button>
		</div>
	`;

	#allClubsView = () => html`
		<div class="header">
			<span>Your clubs</span>
		</div>

		<div class="clubs">
			${this.clubs?.map((c) => html`
					<div
						class="club"
						tabindex="0"
						@click="${() => (this.selectedClub = c)}"
					>
						<img
							src="${c.icon ?? "ph-250.png"}"
							alt="${c.name}"
							width="24"
							height="24"
						/>
						<span>${c.name}</span>
					</div>
				`) ?? "loading..."}
		</div>
	`;

	render() {
		return html`
			<a @click="${() => (this.visible = true)}">Add to folder</a>
			${this.visible
				? html`
						<div
							class="club-folder-selector my-modal"
							v-if="visible"
							@click="${this.#hide}"
						>
							<div class="content">
								${this.selectedClub !== null
									? this.#selectedClubView()
									: this.#allClubsView()}
							</div>
						</div>
				  `
				: ""}
		`;
	}

	#hide = (e: MouseEvent) => {
		if (e.target !== e.currentTarget) return;
		this.visible = false;
	};

	#treeRef = createRef();

	#add = async () => {
		const folderId: number | null = (this.#treeRef.value as FolderTree)
			.selected;

		if (folderId === null) {
			this.status = {
				message: "You must select a folder!",
				success: false,
			};
			return;
		}

		const response = await http.post(
			`${this.foldersRoute}/add-story`,
			{
				folderId: folderId,
				storyId: this.storyId,
			},
			{
				RequestVerificationToken: this.csrf,
			}
		);

		if (response.isSuccess) {
			this.status = {
				message: "Successfully added",
				success: true,
			};
		} else {
			this.status = {
				message: response.error,
				success: false,
			};
		}
	};

	createRenderRoot() {
		return this;
	}
}
