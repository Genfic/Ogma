import { customElement, property, state } from "lit/decorators.js";
import { html, LitElement } from "lit";
import { log } from "../helpers/logger";
import { http } from "../helpers/http";

interface Folder {
	id: number;
	parentFolderId: number | null;
	name: string;
	slug: string;
	canAdd: boolean;
}

interface TreeItem {
	id: number;
	name: string;
	canAdd: boolean;
	children: TreeItem[];
}

@customElement("o-folder-tree")
export class FolderTree extends LitElement {
	constructor() {
		log.info("Tree constructed");
		super();
	}

	@property() clubId: number;
	@property() route: string;
	@property() label: string;
	@property() value: number | null = null;
	@property() current: number | null = null;
	@property() selected: number | null = this.value;
	@property() description: string | null = null;

	@state() private folders: Folder[] = [];
	@state() private tree: TreeItem[] = [];
	@state() private name: string;

	async connectedCallback() {
		log.info("tree connected");
		super.connectedCallback();
		this.classList.add("wc-loaded");

		const response = await http.get<Folder[]>(
			`${this.route}/${this.clubId}`
		);
		if (response.isSuccess) {
			this.folders = response.getValue();
		} else {
			log.error(`Error fetching data: ${response.error}`);
		}

		this.#unflatten();
	}

	#bus = (id: any) => {
		log.info(`Consuming bus with ID ${id} in the tree`);
		this.selected = id;
	};
	
	#name = () => this.label?.replace(/\s+/g, "");

	render() {
		return html`
			<div class="o-form-group">
				${this.label ? html`<label for="${this.#name()}">
					${this.label.replace(/([A-Z])/g, " $1")}
				</label>` : ''}
				
				${this.description ? html`<p class="desc" >${this.description}</p>` : ''}
				
				<input
					type="hidden"
					value="${this.selected}"
					name="${this.name}"
				/>

				<div class="folder-tree active-border">
					${this.tree.length > 0
						? html`
								${this.tree.map(
									(f) => html`
										<o-folder-tree-item
											.folder="${f}"
											@bus="${(e) => this.#bus(e.detail.id)}"
											selected="${this.selected}"
											current="${this.current}"
										>
										</o-folder-tree-item>
									`
								)}
						  `
						: html`<span>No folder found</span>`}

					<template v-else> No folders found</template>
				</div>
			</div>
		`;
	}

	#unflatten = () => {
		let hashTable = Object.create(null);
		this.folders.forEach(
			(aData) => (hashTable[aData.id] = { ...aData, children: [] })
		);
		this.folders.forEach((aData) => {
			if (aData.parentFolderId) {
				hashTable[aData.parentFolderId].children.push(
					hashTable[aData.id]
				);
			} else {
				this.tree.push(hashTable[aData.id]);
			}
		});
	};

	createRenderRoot() {
		return this;
	}
}
