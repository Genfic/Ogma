import type { ComponentOptions, FunctionComponent } from "component-register";
import type { ComponentType } from "solid-element";
import { onMount } from "solid-js";

type SolidElementInstance = Element & { renderRoot?: Node | null };

/**
 * A higher-order component function that wraps a given component and injects scoped CSS styles
 * into its rendering context. The injected CSS is appended to the component's shadow DOM or
 * render root during the component's mount lifecycle.
 *
 * @template TProps The type of the component's props.
 * @param {ComponentType<TProps>} component The component to wrap and style. Can be a functional or class-based component.
 * @param {string} css The CSS styles to be applied to the wrapped component.
 * @returns {ComponentType<TProps>} A new component type with the specified styles applied.
 */
export const Styled = <TProps extends object>(
	component: FunctionComponent<TProps>,
	...css: string[]
): ComponentType<TProps> => {
	return (props: TProps, options: ComponentOptions) => {
		onMount(() => {
			const styleEl = document.createElement("style");
			styleEl.textContent = css.join("");

			const element = options.element as unknown as SolidElementInstance;

			if (element.renderRoot && element.renderRoot !== element) {
				element.renderRoot.appendChild(styleEl);
			} else {
				element.appendChild(styleEl);
			}
		});

		return component(props, options);
	};
};
