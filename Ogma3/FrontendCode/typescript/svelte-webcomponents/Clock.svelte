<svelte:options customElement="my-element" />

<script lang="ts">
	import { onMount } from 'svelte';
	// Assuming these helpers are available in your project
	import { addToDate } from '../src-helpers/date-helpers';
	import { EU, iso8601 } from '../src-helpers/tinytime-templates';

	let { date: initialDate }: { date: Date } = $props();

	// Use $state for reactive date
	let date = $state(initialDate);

	// Use $derived for computed values
	let formattedDate = $derived(EU.render(date));
	let isoDate = $derived(iso8601.render(date));

	onMount(() => {
		const intervalId = setInterval(() => {
			date = addToDate(date, { seconds: 1 });
		}, 1000);

		// Cleanup interval on component destroy
		return () => {
			clearInterval(intervalId);
		};
	});
</script>

<time class="timer" datetime={isoDate} title="Server time">
	{formattedDate}
</time>

<style>
	time {
		font-family: 'Courier New', Courier, monospace;
		letter-spacing: -2px;
		margin: auto 0;
	}
</style>