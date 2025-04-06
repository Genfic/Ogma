<svelte:options customElement="my-element" />

<script lang="ts">
import { log } from "../src-helpers/logger";
import { PostApiReports as postReport } from "../generated/paths-public";
import type Modal from "./Modal.svelte"; // Import the generic modal
import type { ReportReason } from "../types"; // Assuming type definition

// Props
const {
	contentType, // e.g., 'story', 'comment', 'user'
	contentId, // ID of the content being reported
	csrf,
}: {
	contentType: string;
	contentId: number | string;
	csrf: string;
} = $props();

// Assuming reasons are fetched or defined elsewhere
const reportReasons: ReportReason[] = [
	{ id: "spam", text: "Spam or Advertising" },
	{ id: "abuse", text: "Abuse or Harassment" },
	{ id: "illegal", text: "Illegal Content" },
	{ id: "copyright", text: "Copyright Violation" },
	{ id: "other", text: "Other Issue" },
];

// State
let modalRef: Modal | null = $state(null); // Reference to the Modal component instance
let selectedReason = $state<string | null>(null);
let additionalInfo = $state("");
let isSubmitting = $state(false);
let submitError = $state<string | null>(null);
let submitSuccess = $state(false);

async function handleSubmit() {
	if (!selectedReason || isSubmitting) return;

	isSubmitting = true;
	submitError = null;
	submitSuccess = false;

	try {
		const result = await postReport(
			{
				report: {
					target_id: contentId.toString(), // API might expect string
					target_type: contentType,
					reason: selectedReason,
					info: additionalInfo,
				},
			},
			{ RequestVerificationToken: csrf },
		);

		if (result.ok) {
			submitSuccess = true;
			// Optionally close modal after a delay or keep it open with success message
			setTimeout(() => {
				close();
			}, 2000);
		} else {
			log.error(`Failed to submit report: ${result.statusText}`, result.data);
			submitError = result.data?.message || "Failed to submit report. Please try again.";
		}
	} catch (err) {
		log.error("Error submitting report:", err);
		submitError = "An unexpected error occurred. Please try again.";
	} finally {
		isSubmitting = false;
	}
}

// Export functions to control the modal from parent
export function open() {
	submitSuccess = false; // Reset state when opening
	submitError = null;
	selectedReason = null;
	additionalInfo = "";
	modalRef?.openModal();
}
export function close() {
	modalRef?.closeModal();
}
</script>

<Modal bind:this={modalRef} label="Report Content">
	{#if submitSuccess}
		<p>Thank you, your report has been submitted.</p>
	{:else}
		<form on:submit|preventDefault={handleSubmit}>
			<p>Please select a reason for reporting this content:</p>
			<div class="reasons">
				{#each reportReasons as reason (reason.id)}
					<label>
						<input type="radio" name="report-reason" value={reason.id} bind:group={selectedReason} required />
						{reason.text}
					</label>
				{/each}
			</div>

			{#if selectedReason === 'other'}
				<div class="additional-info">
					<label for="report-info">Please provide additional information:</label>
					<textarea id="report-info" bind:value={additionalInfo} rows="3"></textarea>
				</div>
			{/if}

			{#if submitError}
				<p class="error">{submitError}</p>
			{/if}
		</form>

	{/if}


	<div slot="footer">
		<button type="button" class="button secondary" on:click={close} disabled={isSubmitting}>
			Cancel
		</button>
		<button type="submit" class="button primary" disabled={!selectedReason || isSubmitting}>
			{#if isSubmitting}Submitting...{:else}Submit Report{/if}
		</button>
	</div>
</Modal>

<style>
	form {
		display: flex;
		flex-direction: column;
		gap: 1rem;
	}
	.reasons label {
		display: block;
		margin-bottom: 0.5rem;
	}
	.additional-info {
		margin-top: 0.5rem;
	}
	.additional-info label {
		display: block;
		margin-bottom: 0.25rem;
	}
	textarea {
		width: 100%;
		padding: 0.5rem;
		border: 1px solid #ccc;
		border-radius: 4px;
	}
	.error {
		color: red;
		margin-top: 0.5rem;
	}
	/* Add styles for .button, .primary, .secondary if needed */
	.button {
		padding: 8px 15px;
		cursor: pointer;
		border-radius: 4px;
		border: 1px solid transparent;
	}
	.button.primary {
		background-color: var(--theme-color, blue);
		color: white;
	}
	.button.secondary {
		background-color: #eee;
		border-color: #ccc;
		color: #333;
	}
	.button:disabled {
		opacity: 0.7;
		cursor: not-allowed;
	}
</style>