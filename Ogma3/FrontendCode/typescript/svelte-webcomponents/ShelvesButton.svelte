<svelte:options customElement="my-element" />

<script lang="ts">
import { onMount } from "svelte";
import { log } from "../src-helpers/logger";
import {
	GetApiShelfStories as getStoryShelves,
	GetApiShelves as getAllShelves,
	PostApiShelfStories as addToShelf,
	DeleteApiShelfStories as removeFromShelf,
} from "../generated/paths-public";
import Modal from "./Modal.svelte"; // Use the generic modal
import Icon from "./Icon.svelte"; // Use Icon component
import type { Shelf } from "../types"; // Assuming { id: number, name: string }

// Props
let { storyId, csrf }: { storyId: number; csrf: string } = $props();

// State
let modalRef: Modal | null = $state(null);
let allShelves = $state<Shelf[]>([]);
let storyShelfIds = $state<Set<number>>(new Set()); // IDs of shelves this story is on
let isLoadingShelves = $state(true);
let isUpdatingShelf = $state<number | null>(null); // ID of shelf being updated, or null
let error = $state<string | null>(null);

async function loadData() {
	isLoadingShelves = true;
	error = null;
	try {
		const [allShelvesRes, storyShelvesRes] = await Promise.all([getAllShelves(), getStoryShelves(storyId)]);

		if (allShelvesRes.ok) {
			allShelves = allShelvesRes.data ?? [];
		} else {
			log.error("Failed to load all shelves:", allShelvesRes.statusText);
			error = "Could not load shelves.";
		}

		if (storyShelvesRes.ok) {
			storyShelfIds = new Set((storyShelvesRes.data ?? []).map((s) => s.id));
		} else {
			log.error("Failed to load story shelves:", storyShelvesRes.statusText);
			// Don't necessarily set error state here if all shelves loaded ok
		}
	} catch (err) {
		log.error("Error loading shelf data:", err);
		error = "An error occurred while loading shelf data.";
	} finally {
		isLoadingShelves = false;
	}
}

function openShelvesModal() {
	// Load data when modal is opened
	loadData();
	modalRef?.openModal();
}

async function handleShelfToggle(shelfId: number, isCurrentlyOnShelf: boolean) {
	if (isUpdatingShelf !== null) return; // Prevent concurrent updates

	isUpdatingShelf = shelfId;
	error = null; // Clear previous errors

	const action = isCurrentlyOnShelf ? removeFromShelf : addToShelf;
	const params = { shelfId: shelfId, bookId: storyId }; // Assuming bookId corresponds to storyId

	try {
		const result = await action(params, { RequestVerificationToken: csrf });
		if (result.ok) {
			// Update the local state optimistically or based on response
			if (isCurrentlyOnShelf) {
				storyShelfIds.delete(shelfId);
			} else {
				storyShelfIds.add(shelfId);
			}
			// Force reactivity update for the Set
			storyShelfIds = storyShelfIds;
		} else {
			log.error(`Failed to ${isCurrentlyOnShelf ? "remove from" : "add to"} shelf:`, result.statusText);
			error = `Failed to update shelf: ${result.data?.message || result.statusText}`;
		}
	} catch (err) {
		log.error("Error updating shelf:", err);
		error = "An unexpected error occurred.";
	} finally {
		isUpdatingShelf = null;
	}
}
</script>

<button class="button icon-button" title="Add to shelf" on:click={openShelvesModal}>
	<Icon icon="lucide:book-plus" />
</button>

<Modal bind:this={modalRef} label="Manage Shelves">
	<div class="shelf-list">
		{#if isLoadingShelves}
			<p>Loading shelves...</p>
		{:else if error}
			<p class="error">{error}</p>
		{:else if allShelves.length === 0}
			<p>You don't have any shelves yet. <a href="/shelves">Create one?</a></p>
		{:else}
			<ul>
				{#each allShelves as shelf (shelf.id)}
					{@const isOnShelf = storyShelfIds.has(shelf.id)}
					{@const isUpdatingThis = isUpdatingShelf === shelf.id}
					<li>
						<label>
							<input
								type="checkbox"
								checked={isOnShelf}
								disabled={isUpdatingThis}
								on:change={() => handleShelfToggle(shelf.id, isOnShelf)}
							/>
							{shelf.name}
						</label>
						{#if isUpdatingThis}
							<span class="spinner">(updating...)</span>
						{/if}
					</li>
				{/each}
			</ul>
		{/if}
	</div>
	<div slot="footer">
		<button class="button secondary" on:click={() => modalRef?.closeModal()}>
			Done
		</button>
	</div>
</Modal>

<style>
	.button.icon-button {
		/* Styles for the trigger button */
		background: none;
		border: none;
		padding: 5px;
		cursor: pointer;
	}
	.button.icon-button :global(.o-icon) {
		width: 24px;
		height: 24px;
	}

	.shelf-list ul {
		list-style: none;
		padding: 0;
		margin: 0;
		max-height: 300px; /* Example max height */
		overflow-y: auto;
	}
	.shelf-list li {
		padding: 0.5rem 0;
		border-bottom: 1px solid #eee;
		display: flex;
		justify-content: space-between;
		align-items: center;
	}
	.shelf-list li:last-child {
		border-bottom: none;
	}
	.shelf-list label {
		display: flex;
		align-items: center;
		gap: 0.5rem;
		cursor: pointer;
	}
	.spinner {
		font-size: 0.8rem;
		color: #666;
	}
	.error {
		color: red;
		margin-bottom: 1rem;
	}
	/* Add styles for .button, .secondary if needed */
	.button {
		padding: 8px 15px;
		cursor: pointer;
		border-radius: 4px;
		border: 1px solid transparent;
	}
	.button.secondary {
		background-color: #eee;
		border-color: #ccc;
		color: #333;
	}
</style>