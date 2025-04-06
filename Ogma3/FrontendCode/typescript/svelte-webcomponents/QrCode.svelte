<svelte:options customElement="my-element" />

<script lang="ts">
import { onMount } from "svelte";
import { type QrCodeGenerateSvgOptions, renderSVG } from "uqr";

// Props
const {
	text,
	size = 128, // Default size
	alt = "QR Code", // Default alt text
}: {
	text: string;
	size?: number;
	alt?: string;
} = $props();

// State for the QR code data URL
let qrCodeUrl = $state<string | null>(null);
let error = $state<string | null>(null);

// Effect to regenerate QR code when text or size changes
$effect(() => {
	error = null;
	qrCodeUrl = null; // Clear previous QR code while generating
	if (text) {
		QRCode.toDataURL(text, { errorCorrectionLevel: "H", width: size })
			.then((url) => {
				qrCodeUrl = url;
			})
			.catch((err) => {
				console.error("Failed to generate QR code:", err);
				error = "Could not generate QR code.";
			});
	} else {
		error = "No text provided for QR code.";
	}
});
</script>

<div class="qr-code-container">
	{#if error}
		<p class="error">{error}</p>
	{:else if qrCodeUrl}
		{@html renderSVG(qrCodeUrl, { alt })}
	{:else}
		<p>Generating QR code...</p>
	{/if}
</div>

<style>
	.qr-code-container {
		display: inline-block; /* Or block, depending on layout needs */
		padding: 5px; /* Optional padding */
		background-color: white; /* Ensure background for QR code visibility */
	}
	.qr-code-container img {
		display: block; /* Prevents extra space below image */
	}
	.error {
		color: red;
		font-size: 0.9rem;
		text-align: center;
	}
</style>