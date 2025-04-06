<svelte:options customElement="my-element" />

<script lang="ts">
import { onMount } from "svelte";

// State
let progress = $state(0);

function updateProgress() {
	// Calculate scroll progress
	const scrollTop = window.scrollY || document.documentElement.scrollTop;
	const scrollHeight = document.documentElement.scrollHeight;
	const clientHeight = document.documentElement.clientHeight;

	if (scrollHeight <= clientHeight) {
		progress = 100; // Already scrolled to bottom or no scroll needed
	} else {
		const scrolled = (scrollTop / (scrollHeight - clientHeight)) * 100;
		progress = Math.min(100, Math.max(0, scrolled)); // Clamp between 0 and 100
	}
}

onMount(() => {
	// Initial calculation
	updateProgress();

	// Update on scroll and resize
	window.addEventListener("scroll", updateProgress, { passive: true });
	window.addEventListener("resize", updateProgress, { passive: true });

	// Cleanup listeners
	return () => {
		window.removeEventListener("scroll", updateProgress);
		window.removeEventListener("resize", updateProgress);
	};
});
</script>

<div class="read-progress-container" role="progressbar" aria-valuenow={progress} aria-valuemin="0" aria-valuemax="100">
	<div class="read-progress-bar" style:width="{progress}%"></div>
</div>

<style>
	.read-progress-container {
		position: fixed; /* Or absolute/sticky depending on desired behavior */
		top: 0;
		left: 0;
		width: 100%;
		height: 4px; /* Height of the progress bar */
		background-color: #eee; /* Background of the container */
		z-index: 1000;
	}
	.read-progress-bar {
		height: 100%;
		background-color: var(--theme-color, blue); /* Color of the progress bar */
		transition: width 0.1s linear; /* Smooth transition */
		width: 0%; /* Initial width */
	}
</style>