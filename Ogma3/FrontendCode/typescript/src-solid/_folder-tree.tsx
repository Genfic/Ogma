import { GetApiFolders as getClubFolders } from "@g/paths-public";
import type { GetFolderResult } from "@g/types-public";
import { log } from "@h/logger";
import { customElement } from "solid-element";
import { For, Show, createSignal, onMount } from "solid-js";

type Folder = {
	id: number;
	name: string;
	slug: string;
	canAdd: boolean;
};

type TreeItem = Folder & {
	children: TreeItem[];
};

customElement(
	"o-folder-tree",
	{
		clubId: Number,
		value: {
			type: Number,
			default: null,
		},
		current: {
			type: Number,
			default: null,
		},
		selected: {
			type: Number,
			default: null,
		},
		inputSelector: String,
	},
	(props) => {
		const [folders, setFolders] = createSignal<GetFolderResult[]>([]);
		const [tree, setTree] = createSignal<TreeItem[]>([]);
		const [selected, setSelected] = createSignal(props.selected || props.value);
		const [input, setInput] = createSignal<HTMLInputElement>(null);

		onMount(async () => {
			log.info("tree connected");
			props.element.classList.add("wc-loaded");

			if (props.inputSelector !== undefined) {
				const inputElement = document.querySelector<HTMLInputElement>(props.inputSelector);
				setInput(inputElement);
				if (inputElement) {
					inputElement.value = null;
				}
			}

			const response = await getClubFolders(props.clubId);
			if (response.ok) {
				setFolders(response.data);
				unflatten();
			} else {
				log.error(`Error fetching data: ${response.statusText}`);
			}
		});

		const select = (id: number) => {
			log.info(`Selecting folder with ID ${id} in the tree`);
			setSelected(id);
			if (input()) {
				input().value = `${id}`;
			}
		};

		const unflatten = () => {
			const hashTable: { [id: number]: TreeItem } = {};
			const result: TreeItem[] = [];

			// First create all items
			for (const aData of folders()) {
				hashTable[aData.id] = { ...aData, children: [] };
			}

			// Then establish parent-child relationships
			for (const aData of folders()) {
				if (aData.parentFolderId) {
					hashTable[aData.parentFolderId].children.push(hashTable[aData.id]);
				} else {
					result.push(hashTable[aData.id]);
				}
			}

			setTree(result);
		};

		const renderItem = (folder: TreeItem) => {
			const tabindex = folder.id === props.current || !folder.canAdd ? "-1" : "0";
			const isDisabled = folder.id === props.current;
			const isLocked = !folder.canAdd;

			return (
				<div class={`folder ${isDisabled ? "disabled" : ""} ${isLocked ? "locked" : ""}`}>
					<span
						class={selected() === folder.id ? "active" : ""}
						tabindex={tabindex}
						onClick={() => select(folder.id)}
					>
						{folder.name}
					</span>
					<For each={folder.children}>{(child) => renderItem(child)}</For>
				</div>
			);
		};

		return () => (
			<div class="folder-tree active-border">
				<Show when={tree().length > 0} fallback={<span>No folder found</span>}>
					<For each={tree()}>{(folder) => renderItem(folder)}</For>
				</Show>
			</div>
		);
	},
);
