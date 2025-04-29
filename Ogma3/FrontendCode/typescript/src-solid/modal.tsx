import { styled } from "./common/_styled";
import { type ComponentType, customElement, noShadowDOM } from "solid-element";
import { Show, createSignal } from "solid-js";
import css from "./modal.css";

const Modal: ComponentType<{ visible: boolean }> = (props, { element }) => {
	noShadowDOM();

	const [visible, setVisible] = createSignal(props.visible);

	// Expose methods to the element instance
	Object.assign(element, {
		show: () => setVisible(true),
		hide: () => setVisible(false),
		toggle: () => setVisible(!visible()),
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
	styled(css)(Modal),
);
