<svelte:options customElement="my-element" />

<script lang="ts">
import { onMount } from "svelte";
import { log } from "../src-helpers/logger";
import { GetApiNotificationsCount as countNotifications } from "../generated/paths-public";
import Icon from "./Icon.svelte"; // Assuming Icon.svelte

// Reactive state for notification count
let notificationCount = $state<number | null>(null); // Use null for initial loading state
let isLoading = $state(true);

onMount(async () => {
	try {
		const res = await countNotifications();
		if (res.ok) {
			notificationCount = res.data;
		} else {
			log.error(`Failed to fetch notification count: ${res.statusText}`);
			notificationCount = 0; // Set to 0 on error? Or handle differently.
		}
	} catch (error) {
		log.error("Error fetching notification count:", error);
		notificationCount = 0; // Set to 0 on error?
	} finally {
		isLoading = false;
	}
});

// Derived state for display count and title
const displayCount = $derived(() => {
	if (notificationCount === null || notificationCount <= 0) {
		return null;
	}
	return notificationCount <= 99 ? notificationCount.toString() : "99+";
});

const linkTitle = $derived(() => {
	if (notificationCount === null || notificationCount <= 0) {
		return "Notifications";
	}
	return `${notificationCount} notification${notificationCount === 1 ? "" : "s"}`;
});
</script>

<a class="nav-link light notifications-btn" href="/notifications" title={linkTitle}>
	<Icon class="material-icons-outlined" icon="lucide:bell" />
	{#if !isLoading && displayCount !== null}
		<span>{displayCount}</span>
	{/if}
	{#if isLoading}
		<span>...</span>
	{/if}
</a>

<style>
	/* Add styles for .nav-link, .light, .notifications-btn span if needed */
	.notifications-btn {
		position: relative;
		display: inline-flex;
		align-items: center;
	}
	.notifications-btn span {
		position: absolute;
		top: -5px;
		right: -8px;
		background-color: red; /* Example */
		color: white;
		border-radius: 50%;
		padding: 2px 5px;
		font-size: 0.7rem;
		min-width: 16px;
		text-align: center;
		line-height: 1;
	}
	.notifications-btn :global(.o-icon) { /* Style nested component */
		/* Add styles if needed */
	}

</style>