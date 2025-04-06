<svelte:options customElement="my-element" />

<script lang="ts">
import { onMount } from "svelte";
import { log } from "../src-helpers/logger";
import {
	DeleteApiVotes as deleteVote,
	GetApiVotes as getVotes,
	PostApiVotes as postVote,
} from "../generated/paths-public";
import Icon from "./Icon.svelte"; // Assuming Icon.svelte

const { storyId, csrf }: { storyId: number; csrf: string } = $props();

// Reactive state
let voted = $state<boolean | null>(null); // Use null for loading
let score = $state<number | null>(null);
let isLoading = $state(true); // For initial load
let isUpdating = $state(false); // For vote action

onMount(async () => {
	isLoading = true;
	try {
		const result = await getVotes(storyId);
		if (result.ok) {
			const { count, didVote } = result.data;
			score = count;
			voted = didVote;
		} else {
			log.error(`Error fetching initial vote data: ${result.statusText}`);
			// Handle error state, maybe set default values
			score = 0;
			voted = false;
		}
	} catch (error) {
		log.error("Error fetching initial vote data:", error);
		score = 0;
		voted = false;
	} finally {
		isLoading = false;
	}
});

// Derived state
const buttonClass = $derived(`votes action-btn large ${voted ? "active" : ""}`);
const iconName = $derived(voted ? "ic:round-star" : "ic:round-star-border");
const displayScore = $derived(score ?? 0);

async function handleVote() {
	if (isLoading || isUpdating) return; // Prevent action during load or update
	isUpdating = true;

	const action = voted ? deleteVote : postVote;

	try {
		const result = await action({ storyId: storyId }, { RequestVerificationToken: csrf });

		if (result.ok) {
			const { count, didVote } = result.data;
			score = count;
			voted = didVote;
		} else {
			log.error(`Error posting/deleting vote: ${result.statusText}`);
			// Maybe revert state or show error?
		}
	} catch (error) {
		log.error("Error posting/deleting vote:", error);
	} finally {
		isUpdating = false;
	}
}
</script>

<button
	class={buttonClass}
	onclick={handleVote}
	title="Give it a star!"
	disabled={isLoading || isUpdating}
>
	{#if isLoading}
		<span class="count">...</span>
	{:else}
		<Icon icon={iconName} class="material-icons-outlined" />
		<span class="count">{displayScore}</span>
	{/if}
</button>

<style>
	/* Add styles for .votes, .action-btn, .large, .active, .count */
	button {
		display: inline-flex;
		align-items: center;
		gap: 4px;
		padding: 8px 12px;
		cursor: pointer;
	}
	button:disabled {
		opacity: 0.7;
		cursor: wait;
	}
	.active {
		/* Example style for active state */
		color: gold;
	}
	.count {
		font-weight: bold;
	}
	.votes :global(.o-icon) { /* Style nested component */
		/* Add styles if needed */
	}
</style>