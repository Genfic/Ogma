<svelte:options customElement="my-element" />

<script lang="ts">
	import { onMount } from 'svelte';
	import { log } from '../src-helpers/logger'; // Assuming helper exists
	import { GetApiQuotesRandom as getQuote } from '../generated/paths-public'; // Assuming API function exists
	import Icon from './Icon.svelte'; // Assuming Icon.svelte exists

	interface Quote {
		body: string;
		author: string;
	}

	// Reactive state
	let loading = $state(true);
	let quote = $state<Quote | null>(null);
	let canReload = $state(true); // Controls the reload button availability/icon

	// Derived state for spinner icon and class
	let spinnerIcon = $derived(canReload ? 'lucide:refresh-cw' : 'lucide:clock');
	let spinnerClass = $derived(loading ? 'spin' : '');

	async function loadQuote() {
		// Don't reload if in cooldown or already loading
		if (!canReload || loading) {
			return;
		}

		loading = true;

		try {
			const response = await getQuote();

			if (response.ok) {
				quote = response.data;
				// Cache the quote in localStorage
				try {
					window.localStorage.setItem('quote', JSON.stringify(quote));
				} catch (e) {
					log.warn('Could not save quote to localStorage', e);
				}
			} else if (response.status === 429) {
				log.warn('Rate limit hit (429), attempting to load quote from localStorage.');
				// Attempt to load from localStorage on rate limit
				try {
					const storedQuote = window.localStorage.getItem('quote');
					if (storedQuote) {
						quote = JSON.parse(storedQuote);
					} else {
						log.warn('No quote found in localStorage.');
						// Handle case where there's no fallback?
						quote = { body: 'Rate limited. Please try again later.', author: 'System' };
					}
				} catch (e) {
					log.error('Failed to parse quote from localStorage', e);
					quote = { body: 'Error loading cached quote.', author: 'System' };
				}
			} else {
				log.error(`Failed to fetch quote: ${response.statusText}`);
				// Consider loading from cache here too, or showing an error state
				quote = { body: 'Could not load quote.', author: 'System' };
			}

			// Handle cooldown based on Retry-After header (if present and valid)
			const retryAfterHeader = response.headers?.get('retry-after');
			const retryAfterSeconds = retryAfterHeader ? parseInt(retryAfterHeader, 10) : 60; // Default cooldown (e.g., 60s)

			if (!isNaN(retryAfterSeconds) && retryAfterSeconds > 0) {
				canReload = false;
				setTimeout(() => {
					canReload = true;
				}, retryAfterSeconds * 1000);
			} else {
				// If no valid Retry-After, enable reload immediately or after a default short delay
				canReload = true;
			}

		} catch (error) {
			log.error('Error during quote fetch:', error);
			quote = { body: 'An error occurred while fetching the quote.', author: 'System' };
			canReload = true; // Allow retry on generic error
		} finally {
			loading = false;
		}
	}

	onMount(() => {
		// Attempt initial load from localStorage first, then fetch
		try {
			const storedQuote = window.localStorage.getItem('quote');
			if (storedQuote) {
				quote = JSON.parse(storedQuote);
				loading = false; // Assume cached quote is good enough initially
				// Still trigger a background fetch? Depends on desired freshness.
				// loadQuote(); // Optional: uncomment to refresh in background
			} else {
				// No cached quote, fetch immediately
				loadQuote();
			}
		} catch (e) {
			log.error('Failed to parse initial quote from localStorage', e);
			// Fetch if localStorage fails
			loadQuote();
		}
	});

</script>

<div class="quote-box"> <div id="quote" class="quote active-border">
	<div class="refresh" on:click={loadQuote} title={canReload ? 'Get new quote' : 'Please wait'}>
		<Icon icon={spinnerIcon} class="material-icons-outlined {spinnerClass}" />
	</div>

	{#if quote}
		<em class="body">{quote.body}</em>
		<span class="author">{quote.author}</span>
	{:else if loading}
		<span>Loading the quote...</span>
	{:else}
		<span>Could not load quote.</span> {/if}
</div>
</div>

<style>
	.quote-box {
		/* Styles for the outer container if needed */
	}
	.quote {
		position: relative; /* For positioning the refresh button */
		padding: 1rem;
		border: 1px solid #ccc; /* Example border */
		border-radius: 4px;
		background-color: #f9f9f9;
	}
	.active-border {
		/* Add specific styles if needed */
	}
	.refresh {
		position: absolute;
		top: 5px;
		right: 5px;
		cursor: pointer;
		opacity: 0.7;
		transition: opacity 0.2s;
	}
	.refresh:hover {
		opacity: 1;
	}
	.refresh :global(.o-icon) { /* Target the nested icon component */
		font-size: 1.2rem;
	}
	.spin {
		animation: spin 1s linear infinite;
	}
	@keyframes spin {
		from { transform: rotate(0deg); }
		to { transform: rotate(360deg); }
	}
	.body {
		display: block;
		font-style: italic;
		margin-bottom: 0.5rem;
	}
	.author {
		display: block;
		text-align: right;
		font-size: 0.9rem;
		color: #555;
	}
	.author::before {
		content: "— ";
	}
</style>