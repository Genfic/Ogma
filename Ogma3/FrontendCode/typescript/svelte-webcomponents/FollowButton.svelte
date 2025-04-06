<svelte:options customElement="my-element" />

<script lang="ts">
import { log } from "../src-helpers/logger";
import { PostApiUsersFollow as followUser, DeleteApiUsersFollow as unfollowUser } from "../generated/paths-public";

const {
	userName,
	csrf,
	isFollowed: initialIsFollowed,
}: {
	userName: string;
	csrf: string;
	isFollowed: boolean;
} = $props();

// Reactive state for followed status and loading
let isFollowed = $state(initialIsFollowed);
let isLoading = $state(false);

// Derived state for button text and title
const buttonText = $derived(isFollowed ? "Following" : "Follow");
const buttonTitle = $derived(isFollowed ? "Unfollow" : "Follow");
const buttonClass = $derived(`button max ${isFollowed ? "leave" : "join"}`);

async function handleFollowToggle() {
	if (isLoading) return;
	isLoading = true;

	const action = isFollowed ? unfollowUser : followUser;

	try {
		const res = await action({ name: userName }, { RequestVerificationToken: csrf });

		if (res.ok) {
			isFollowed = res.data;
		} else {
			log.warn(`Failed to ${isFollowed ? "unfollow" : "follow"} user: ${res.statusText}`);
		}
	} catch (error) {
		log.error("Error during follow/unfollow action:", error);
	} finally {
		isLoading = false;
	}
}
</script>

<button
	class={buttonClass}
	title={buttonTitle}
	onclick={handleFollowToggle}
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
		/* Example styles */
		padding: 8px 15px;
		cursor: pointer;
	}
	button:disabled {
		opacity: 0.7;
		cursor: wait;
	}
</style>