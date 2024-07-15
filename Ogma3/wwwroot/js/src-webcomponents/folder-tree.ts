import { html, LitElement } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { classMap } from "lit/directives/class-map.js";
import { Folders_GetFoldersOfClub as getClubFolders } from "../generated/paths-public";
import { log } from "../src-helpers/logger";

type Folder = {
	id: number;
	parentFolderId: number | null;
	name: string;
	slug: string;
	canAdd: boolean;
};

type TreeItem = {
	id: number;
	name: string;
	canAdd: boolean;
	children: TreeItem[];
};

@customElement("o-folder-tree")
export class FolderTree extends LitElement {
	constructor() {
		super();
	}

	@property() accessor clubId: number;
	@property() accessor value: number | null = null;
	@property() accessor current: number | null = null;
	@property() accessor selected: number | null = this.value;
	@property() accessor inputSelector: string | undefined = undefined;

	@state() private accessor folders: Folder[] = [];
	@state() private accessor tree: TreeItem[] = [];
	@state() private accessor name: string;
	@state() private accessor input: HTMLInputElement;

	async connectedCallback() {
		log.info("tree connected");
		super.connectedCallback();
		this.classList.add("wc-loaded");

		if (this.inputSelector !== undefined) {
			this.input = document.querySelector<HTMLInputElement>(this.inputSelector);
			this.input.value = null;
		}

		const response = await getClubFolders(this.clubId);
		if (response.ok) {
			this.folders = await response.json();
		} else {
			log.error(`Error fetching data: ${response.statusText}`);
		}

		this.#unflatten();
	}

	#select = (id: number) => {
		log.info(`Selecting folder with ID ${id} in the tree`);
		this.selected = id;
		this.input.value = `${id}`;
	};

	render() {
		return html`
			<div class="folder-tree active-border">
				${this.tree.length > 0 ? html` ${this.tree.map(this.#item)} ` : html`<span>No folder found</span>`}
			</div>
		`;
	}

	#item = (folder: TreeItem) => {
		const tabindex = folder.id === this.current || !folder.canAdd ? "-1" : "0";

		const classes = classMap({
			disabled: folder.id === this.current,
			locked: !folder.canAdd,
		});

		return html`
			<div class="folder ${classes}">
				<span
					class="${this.selected === folder.id ? "active" : ""}"
					tabindex="${tabindex}"
					@click="${() => this.#select(folder.id)}"
				>
					${folder.name}
				</span>
				${folder.children?.map(this.#item) ?? ""}
			</div>
		`;
	};

	#unflatten = () => {
		const hashTable = Object.create(null);

		for (const aData of this.folders) {
			hashTable[aData.id] = { ...aData, children: [] };
		}

		for (const aData of this.folders) {
			if (aData.parentFolderId) {
				hashTable[aData.parentFolderId].children.push(hashTable[aData.id]);
			} else {
				this.tree.push(hashTable[aData.id]);
			}
		}
	};

	createRenderRoot() {
		return this;
	}
}
