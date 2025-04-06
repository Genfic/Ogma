<svelte:options customElement="my-element" />

<script lang="ts">
import { log } from "../src-helpers/logger"; // Assuming helper is available
import { PostApiUsersBlock as blockUser, DeleteApiUsersBlock as unblockUser } from "../generated/paths-public"; // Assuming API functions are available

// Define props
const {
	userName,
	csrf,
	isBlocked: initialIsBlocked,
}: {
	userName: string;
	csrf: string;
	isBlocked: boolean;
} = $props();

// Use $state for internal reactive state, initialized from the prop
let isBlocked = $state(initialIsBlocked);
let isLoading = $state(false); // Optional: track loading state

// $derived state for button text
const buttonText = $derived(isBlocked ? "Unblock" : "Block");

async function handleBlockToggle() {
	if (isLoading) return; // Prevent multiple clicks while loading
	isLoading = true;

	const action = isBlocked ? unblockUser : blockUser;

	try {
		const res = await action({ name: userName }, { RequestVerificationToken: csrf });

		if (res.ok) {
			// Update the reactive state
			isBlocked = res.data;
		} else {
			log.warn(`Failed to ${isBlocked ? "unblock" : "block"} user: ${res.statusText}`);
			// Optionally show an error message to the user
		}
	} catch (error) {
		log.error("Error during block/unblock action:", error);
		// Optionally show an error message to the user
	} finally {
		isLoading = false;
	}
}

function handleKeydown(event: KeyboardEvent) {
	if (event.key === " ") {
		event.preventDefault();
		handleBlockToggle();
	}
}
</script>

<span
	role="button"
	tabindex="0"
	on:click={handleBlockToggle}
	on:keydown={handleKeydown}
	aria-disabled={isLoading}
	style:cursor={isLoading ? 'wait' : 'pointer'}
>
	{buttonText}
	{#if isLoading}
		(Updating...)
	{/if}
</span>

<style>
	/* Add any necessary styles here */
	span[role="button"] {
		/* Basic button styling */
		display: inline-block;
		padding: 4px 8px;
		border: 1px solid #ccc;
		border-radius: 4px;
		user-select: none;
	}
	span[aria-disabled="true"] {
		opacity: 0.6;
	}
</style>