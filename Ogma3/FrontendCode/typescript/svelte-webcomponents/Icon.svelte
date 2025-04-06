<svelte:options customElement="my-element" />

<script lang="ts">
const { icon }: { icon: string } = $props();
</script>

<svg width="24" height="24" class="o-icon" part="icon">
	<use href="/svg/spritesheet.svg#{icon}"></use>
</svg>

<style>
	/* Add any specific styles for o-icon here if needed */
	.o-icon {
		display: inline-block; /* Example style */
		vertical-align: middle;
	}
</style>