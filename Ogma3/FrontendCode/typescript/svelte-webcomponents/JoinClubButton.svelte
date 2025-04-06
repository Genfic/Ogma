<svelte:options customElement="my-element" />

<script lang="ts">
import { log } from "../src-helpers/logger";
import { PostApiClubjoin as joinClub, DeleteApiClubjoin as leaveClub } from "../generated/paths-public";

const {
	clubId,
	csrf,
	isMember: initialIsMember,
}: {
	clubId: number;
	csrf: string;
	isMember: boolean;
} = $props();

// Reactive state
let isMember = $state(initialIsMember);
let isLoading = $state(false);

// Derived state
const buttonText = $derived(isMember ? "Leave club" : "Join club");
const buttonTitle = $derived(isMember ? "Leave" : "Join");
const buttonClass = $derived(`button max ${isMember ? "leave" : "join"}`);

async function handleJoinToggle() {
	if (isLoading) return;
	isLoading = true;

	const action = isMember ? leaveClub : joinClub;

	try {
		const res = await action({ clubId: clubId }, { RequestVerificationToken: csrf });

		if (res.ok) {
			isMember = res.data === true;
		} else {
			log.warn(`Failed to ${isMember ? "leave" : "join"} club: ${res.statusText}`);
		}
	} catch (error) {
		log.error("Error during join/leave action:", error);
	} finally {
		isLoading = false;
	}
}
</script>

<button
	class={buttonClass}
	title={buttonTitle}
	onclick={handleJoinToggle}
	disabled={isLoading}
>
	{buttonText}
	{#if isLoading}
		...
	{/if}
</button>

<style>
	/* Add styles for .button, .max, .leave, .join if needed */
	button {
		padding: 8px 15px;
		cursor: pointer;
	}
	button:disabled {
		opacity: 0.7;
		cursor: wait;
	}
</style>