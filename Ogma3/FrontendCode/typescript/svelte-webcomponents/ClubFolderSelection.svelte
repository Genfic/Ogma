<svelte:options customElement="my-element" />

<script lang="ts">
import { onMount } from "svelte";
import { log } from "../src-helpers/logger";
import { GetApiFolders as getClubFolders } from "../generated/paths-public";
import type { Folder } from "../types"; // Assuming shared Folder type
import FolderTree from "./FolderTree.svelte"; // Import the tree component

// Props
const { clubId, initialFolderId = null }: { clubId: number; initialFolderId?: number | null } = $props();

// Reactive state
let folders = $state<Folder[]>([]);
let selectedFolderId = $state<number | null>(initialFolderId);
let isLoading = $state(true);
let error = $state<string | null>(null);

// Fetch folders on mount
onMount(async () => {
	isLoading = true;
	error = null;
	try {
		const result = await getClubFolders(clubId);
		if (result.ok) {
			// Assuming result.data is the array of Folder objects
			folders = result.data;
		} else {
			log.error(`Failed to fetch club folders: ${result.statusText}`);
			error = `Failed to load folders: ${result.statusText}`;
		}
	} catch (err) {
		log.error("Error fetching club folders:", err);
		error = "An error occurred while loading folders.";
	} finally {
		isLoading = false;
	}
});

// Handle selection event from FolderTree
function handleFolderSelect(event: CustomEvent<number>) {
	selectedFolderId = event.detail;
	console.log("Selected folder ID:", selectedFolderId);
	// Potentially dispatch another event or perform an action here
}

// Expose selected ID (could be a derived state or function)
export function getSelectedFolderId() {
	return selectedFolderId;
}
</script>

<div class="club-folder-selector">
	{#if isLoading}
		<p>Loading folders...</p>
	{:else if error}
		<p class="error">{error}</p>
	{:else if folders.length === 0}
		<p>No folders available.</p>
	{:else}
		<FolderTree
			{folders}
			{selectedFolderId}
			on:select={handleFolderSelect}
		/>
	{/if}

	{#if selectedFolderId !== null}
		<p>Selected: Folder {selectedFolderId}</p>
	{/if}
</div>

<style>
	.club-folder-selector {
		/* Add styles for the container */
		border: 1px solid #ccc;
		padding: 1rem;
		min-height: 100px; /* Example */
	}
	.error {
		color: red;
	}
</style>