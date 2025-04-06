<svelte:options customElement="my-element" />

<script lang="ts">
import { tick } from "svelte"; // For focus management
import { trapFocus } from "../src-helpers/trap-focus"; // Assuming helper exists
import Icon from "./Icon.svelte"; // Assuming Icon.svelte

// Props
const {
	label = "Dialog window", // Default label
	open = false, // Prop to control visibility externally (can be bound)
	closeOnEscape = true,
	closeOnBackdropClick = true,
}: {
	label?: string;
	open?: boolean;
	closeOnEscape?: boolean;
	closeOnBackdropClick?: boolean;
} = $props();

// Reactive state derived from prop, allowing internal control too
let isOpen = $state(open);
let modalElement: HTMLElement | undefined = $state();
let previouslyFocusedElement: HTMLElement | null = $state(null);

// Effect to handle prop changes and focus management
$effect(() => {
	// Sync internal state if prop changes externally
	if (open !== isOpen) {
		isOpen = open;
	}

	if (isOpen && modalElement) {
		// Save previously focused element and trap focus
		previouslyFocusedElement = document.activeElement as HTMLElement;
		// Need to wait for modal to be fully rendered before trapping focus
		tick().then(() => {
			const focusableElement = modalElement?.querySelector(
				'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])',
			) as HTMLElement;
			if (focusableElement) {
				focusableElement.focus();
			} else {
				// Fallback focus if no focusable elements found
				modalElement?.focus();
			}
			// trapFocus helper needs to be adapted for Svelte element refs if necessary
			// trapFocus(modalElement); // Might need adjustment
		});

		// Add escape listener
		if (closeOnEscape) {
			document.addEventListener("keydown", handleKeyDown);
		}
	} else {
		// Cleanup: remove listener and restore focus
		if (closeOnEscape) {
			document.removeEventListener("keydown", handleKeyDown);
		}
		previouslyFocusedElement?.focus();
	}

	// Effect cleanup
	return () => {
		if (closeOnEscape) {
			document.removeEventListener("keydown", handleKeyDown);
		}
	};
});

function handleKeyDown(event: KeyboardEvent) {
	if (event.key === "Escape") {
		closeModal();
	}
}

function handleBackdropClick() {
	if (closeOnBackdropClick) {
		closeModal();
	}
}

export function closeModal() {
	if (!isOpen) return;
	isOpen = false;
	// Update the bindable prop if used
	if (open !== isOpen) {
		// This assumes the parent uses bind:open={modalOpenState}
		// In Svelte 5, mutations flow one way, so parent needs to handle the close event.
		// We can dispatch an event instead.
		// dispatch('close'); // Requires creating dispatcher: const dispatch = createEventDispatcher();
	}
	// For rune props, the parent directly controls `open`. We just update internal state.
	// To notify parent to change the prop, we need an event or callback prop.
	// This example assumes parent updates 'open' prop based on interaction/event.
}

export function openModal() {
	if (isOpen) return;
	isOpen = true;
	// Similar logic for notifying parent if needed
	// dispatch('open');
}
</script>

{#if isOpen}
	<div
		class="modal-backdrop"
		on:click={handleBackdropClick}
	>
		<div
			bind:this={modalElement}
			class="modal-content"
			role="dialog"
			aria-modal="true"
			aria-label={label}
			tabindex="-1"
			on:click|stopPropagation={() => {}} >
			<header class="modal-header">
				<slot name="header"><h2>{label}</h2></slot>
				<button aria-label="Close dialog" class="close-button" on:click={closeModal}>
				<Icon icon="lucide:x" />
			</button>
			</header>
			<main class="modal-body">
				<slot></slot>
			</main>
			<footer class="modal-footer">
				<slot name="footer"></slot>
			</footer>
		</div>
	</div>
{/if}

<style>
	.modal-backdrop {
		position: fixed;
		inset: 0;
		background-color: rgba(0, 0, 0, 0.5);
		display: flex;
		align-items: center;
		justify-content: center;
		z-index: 1000;
		padding: 1rem;
	}
	.modal-content {
		background-color: white;
		padding: 1.5rem;
		border-radius: 8px;
		max-width: 90vw;
		max-height: 90vh;
		overflow-y: auto;
		position: relative;
	}
	.modal-header {
		display: flex;
		justify-content: space-between;
		align-items: center;
		border-bottom: 1px solid #eee;
		padding-bottom: 0.5rem;
		margin-bottom: 1rem;
	}
	.modal-header h2 {
		margin: 0;
		font-size: 1.25rem;
	}
	.close-button {
		background: none;
		border: none;
		font-size: 1.5rem;
		cursor: pointer;
		padding: 0;
		line-height: 1;
	}
	.modal-body {
		margin-bottom: 1rem;
	}
	.modal-footer {
		border-top: 1px solid #eee;
		padding-top: 1rem;
		margin-top: 1rem;
		display: flex;
		justify-content: flex-end;
		gap: 0.5rem;
	}

	.close-button :global(.o-icon) { /* Style nested component */
		/* Add styles if needed */
	}
</style>