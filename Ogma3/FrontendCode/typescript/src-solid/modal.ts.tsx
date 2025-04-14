import { addStyle } from "@h/jsx-wc-style";
import { type ComponentType, customElement, noShadowDOM } from "solid-element";
import { Show, createSignal, onMount } from "solid-js";
import { minifyCss } from "@h/minify.macro" with { type: "macro" };

const Modal: ComponentType<{ visible: boolean }> = (props, { element }) => {
	noShadowDOM();

	const [visible, setVisible] = createSignal(props.visible);

	// Expose methods to the element instance
	Object.assign(element, {
		show: () => setVisible(true),
		hide: () => setVisible(false),
		toggle: () => setVisible(!visible()),
	});

	// Add styles to shadow root
	onMount(() => {
		addStyle(
			element,
			minifyCss(`
			.my-modal {
				position: fixed;
				display: flex;
				align-items: center;
				justify-content: center;
				inset: 0;
				width: 100%;
				height: 100%;
				z-index: 999;
				background: var(--foreground-50);
			}

			.content {
				min-width: 20rem;
				width: min(40rem, 100vw);
				max-height: 30rem;
				background: var(--background);
				padding: 1rem;
				height: 100%;
				overflow-y: auto;
				border: 5px solid var(--background);
			}`),
		);
	});

	return (
		<Show when={visible()}>
			<div class="my-modal" onClick={() => setVisible(false)}>
				<div class="content" onClick={(e) => e.stopPropagation()}>
					<slot />
				</div>
			</div>
		</Show>
	);
};

customElement(
	"o-modal",
	{
		visible: false,
	},
	Modal,
);
