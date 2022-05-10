import { customElement, property, state } from "lit/decorators.js";
import { html, LitElement } from "lit";
import { classMap } from "lit/directives/class-map.js";
import { log } from "../helpers/logger";

interface Folder {
	id: number;
	name: string;
	canAdd: boolean;
	children: Folder[];
}

@customElement("o-folder-tree-item")
export class FolderTreeItem extends LitElement {
	constructor() {
		super();
	}

	@property() folder: Folder;
	@property() selected: number;
	@property() current: number;
	@state() private disabled: boolean;

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");
	}

	#bus = (id: number) => {
		const busEvent = new CustomEvent("bus", {
			bubbles: true,
			composed: true,
			detail: { id: id },
		});
		
		log.info(`Dispatching bus with ID ${id}`);
		if (id !== this.current) {
			this.dispatchEvent(busEvent);
		}
	};

	render() {
		return html`
			<div class="folder ${this.#folderClasses()}">
				<span
					class="${this.selected === this.folder.id ? "active" : ""}"
					tabindex="${this.#tabindex()}"
					@click="${() => this.#bus(this.folder.id)}"
				>
					${this.folder.name}
				</span>
				${this.folder.children?.map(
			(c) => html`
						<o-folder-tree-item
							.folder="${c}"
							@bus="${(e) => this.#bus(e.detail.id)}"
							selected="${this.selected}" 
							current="${this.current}"
							disabled="${this.folder.id === this.current}"
						></o-folder-tree-item>
					`
		) ?? ''}
			</div>
		`;
	}

	#tabindex = () =>
		this.folder.id === this.current || this.disabled || !this.folder.canAdd
			? "-1"
			: "0";

	#folderClasses = () =>
		classMap({
			disabled: this.folder.id === this.current || this.disabled,
			locked: !this.folder.canAdd,
		});

	createRenderRoot() {
		return this;
	}
}
