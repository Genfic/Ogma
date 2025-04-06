<svelte:options customElement="my-element" />

<script lang="ts">
	import Icon from './Icon.svelte'; // Assuming Icon.svelte is in the same directory

	let { route, chapterId }: { route: string; chapterId: number } = $props();

	// Add any state or logic needed for read status interaction here
	// For example:
	// let isRead = $state(false);
	//
	// function toggleReadStatus() {
	//    // Call API using route and chapterId
	//    console.log(`Toggling read status for chapter ${chapterId}`);
	//    isRead = !isRead;
	// }

</script>

<button
	class="read-status"
	aria-label="Chapter read status"
	data-id={chapterId}
>
	<Icon icon="lucide:eye-off" />
</button>

<style>
	/* Add styles for .read-status if needed */
</style>