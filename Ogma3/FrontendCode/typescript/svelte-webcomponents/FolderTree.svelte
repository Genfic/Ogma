<svelte:options customElement="my-element" />

<script lang="ts">
import { createEventDispatcher } from "svelte"; // Needed for events
import type { Folder } from "../types"; // Assuming Folder type definition
import Icon from "./Icon.svelte"; // Assuming Icon.svelte

const dispatch = createEventDispatcher<{ select: number }>();

// Props
const {
	folders,
	level = 0, // Track nesting level for indentation
	selectedFolderId = null,
}: {
	folders: Folder[];
	level?: number;
	selectedFolderId?: number | null;
} = $props();

// Reactive state for tracking open folders (if collapsible)
let openFolders = $state<Set<number>>(new Set());

function toggleFolder(folderId: number) {
	openFolders.has(folderId) ? openFolders.delete(folderId) : openFolders.add(folderId);
	// Trigger redraw if needed, though Svelte handles set mutations reactively
	openFolders = openFolders;
}

function handleSelectFolder(folderId: number) {
	dispatch("select", folderId);
	// Optionally close other branches or handle selection visuals here
}
</script>

<ul class="folder-tree level-{level}" style:padding-left="{level * 1.5}rem">
	{#each folders as folder (folder.id)}
		<li class:selected={folder.id === selectedFolderId}>
			<div class="folder-item">
				{#if folder.children && folder.children.length > 0}
					<button
						class="toggle-button"
						aria-label={openFolders.has(folder.id) ? 'Collapse' : 'Expand'}
						aria-expanded={openFolders.has(folder.id)}
						on:click={() => toggleFolder(folder.id)}
					>
						<Icon icon={openFolders.has(folder.id) ? 'lucide:chevron-down' : 'lucide:chevron-right'} />
					</button>
				{/if}
				<span
					role="button"
					tabindex="0"
					class="folder-name"
					on:click={() => handleSelectFolder(folder.id)}
					on:keydown={(e) => e.key === 'Enter' || e.key === ' ' ? handleSelectFolder(folder.id) : null}
				>
					<Icon icon="lucide:folder" class="folder-icon" />
					{folder.name}
				</span>
			</div>

			{#if folder.children && folder.children.length > 0 && openFolders.has(folder.id)}
				<svelte:self
					folders={folder.children}
					level={level + 1}
					{selectedFolderId}
					on:select={(event) => dispatch('select', event.detail)}
				/>
			{/if}
		</li>
	{/each}
</ul>

<style>
	.folder-tree {
		list-style: none;
		padding-left: 0; /* Base padding handled by level */
		margin: 0;
	}
	.folder-tree li {
		margin: 0.25rem 0;
	}
	.folder-item {
		display: flex;
		align-items: center;
		gap: 0.25rem;
		cursor: default; /* Default cursor, specific items are clickable */
	}
	.folder-name {
		cursor: pointer;
		padding: 2px 4px;
		border-radius: 3px;
		display: inline-flex;
		align-items: center;
		gap: 0.25rem;
	}
	.folder-name:hover,
	.folder-name:focus {
		background-color: #eee;
	}
	li.selected > .folder-item > .folder-name {
		background-color: #ddd;
		font-weight: bold;
	}
	.toggle-button {
		background: none;
		border: none;
		padding: 0;
		cursor: pointer;
		display: inline-flex;
		align-items: center;
		justify-content: center;
		width: 1.5rem; /* Ensure consistent size */
		height: 1.5rem;
	}
	.folder-icon {
		color: #666; /* Example color */
	}
	.toggle-button :global(.o-icon),
	.folder-icon :global(.o-icon) { /* Style nested component */
		width: 1em;
		height: 1em;
	}

</style>