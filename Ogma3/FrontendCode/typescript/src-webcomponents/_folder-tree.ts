import { GetApiFolders as getClubFolders } from "@g/paths-public";
import type { GetFolderResult } from "@g/types-public";
import { log } from "@h/logger";
import { LitElement, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { classMap } from "lit/directives/class-map.js";

type Folder = {
	id: number;
	name: string;
	slug: string;
	canAdd: boolean;
};

type TreeItem = Folder & {
	children: TreeItem[];
};

@customElement("o-folder-tree")
export class _folderTree extends LitElement {
	@property() accessor clubId: number;
	@property() accessor value: number | null = null;
	@property() accessor current: number | null = null;
	@property() accessor selected: number | null = this.value;
	@property() accessor inputSelector: string | undefined = undefined;

	@state() private accessor folders: GetFolderResult[] = [];
	@state() private accessor tree: TreeItem[] = [];
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
			this.folders = response.data;
		} else {
			log.error(`Error fetching data: ${response.statusText}`);
		}

		this.unflatten();
	}

	private select = (id: number) => {
		log.info(`Selecting folder with ID ${id} in the tree`);
		this.selected = id;
		this.input.value = `${id}`;
	};

	render() {
		return html`
			<div class="folder-tree active-border">
				${this.tree.length > 0 ? html` ${this.tree.map(this.item)} ` : html`<span>No folder found</span>`}
			</div>
		`;
	}

	private item = (folder: TreeItem) => {
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
					@click="${() => this.select(folder.id)}"
				>
					${folder.name}
				</span>
				${folder.children?.map(this.item) ?? ""}
			</div>
		`;
	};

	private unflatten = () => {
		// const hashTable: { [id: number]: TreeItem } = {};
		//
		// for (const aData of this.folders) {
		// 	hashTable[aData.id] = { ...aData, children: [] };
		// }
		//
		// for (const aData of this.folders) {
		// 	if (aData.parentFolderId) {
		// 		hashTable[aData.parentFolderId].children.push(hashTable[aData.id]);
		// 	} else {
		// 		this.tree.push(hashTable[aData.id]);
		// 	}
		// }
	};

	createRenderRoot() {
		return this;
	}
}
