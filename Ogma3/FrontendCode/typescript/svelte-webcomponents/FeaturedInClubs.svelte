<svelte:options customElement="my-element" />

<script lang="ts">
import { onMount } from "svelte";
import { log } from "../src-helpers/logger";
import { GetApiClubsStory as getFeaturedClubs } from "../generated/paths-public";
import type { ClubLink } from "../types"; // Assuming type { id: number, name: string, slug: string }

// Props
const { storyId }: { storyId: number } = $props();

// Reactive state
let clubs = $state<ClubLink[]>([]);
let isLoading = $state(true);
let error = $state<string | null>(null);

onMount(async () => {
	isLoading = true;
	error = null;
	try {
		const result = await getFeaturedClubs(storyId);
		if (result.ok) {
			// Assuming result.data is the array of ClubLink objects
			clubs = result.data;
		} else {
			log.warn(`Failed to fetch featured clubs: ${result.statusText}`);
			// Don't show error, just show nothing if fetch fails? Or set error state.
			// error = `Failed to load clubs: ${result.statusText}`;
		}
	} catch (err) {
		log.error("Error fetching featured clubs:", err);
		// error = 'An error occurred while loading clubs.';
	} finally {
		isLoading = false;
	}
});
</script>

<div class="featured-in-clubs">
	{#if isLoading}
	{:else if error}
		<p class="error">{error}</p>
	{:else if clubs.length > 0}
		<h4>Featured In Clubs</h4>
		<ul>
			{#each clubs as club (club.id)}
				<li><a href="/clubs/{club.slug}">{club.name}</a></li>
			{/each}
		</ul>
	{/if}
</div>

<style>
	.featured-in-clubs {
		margin-top: 1rem;
		padding-top: 1rem;
		border-top: 1px solid #eee; /* Example style */
	}
	.featured-in-clubs h4 {
		margin-top: 0;
		margin-bottom: 0.5rem;
		font-size: 1rem;
		color: #555;
	}
	.featured-in-clubs ul {
		list-style: none;
		padding: 0;
		margin: 0;
		display: flex;
		flex-wrap: wrap;
		gap: 0.5rem;
	}
	.featured-in-clubs li a {
		text-decoration: none;
		color: var(--link-color, blue); /* Use CSS variable or default */
		background-color: #f0f0f0; /* Example style */
		padding: 0.25rem 0.5rem;
		border-radius: 4px;
		font-size: 0.9rem;
	}
	.featured-in-clubs li a:hover {
		background-color: #e0e0e0;
	}
	.error {
		color: red;
		font-size: 0.9rem;
	}
</style>