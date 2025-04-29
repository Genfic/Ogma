import type { FunctionComponent } from "component-register";
import type { ComponentType } from "solid-element";
import { onMount } from "solid-js";

/**
 * Higher-Order Component (HOC) factory to apply styles to a solid-element's renderRoot.
 * @param css The CSS string to apply.
 * @returns A function that takes a ComponentType and returns a new ComponentType with styles applied.
 */
export const styled = <P>(css: string) => {
	return (Component: FunctionComponent<P>): ComponentType<P> => {
		return (props, context) => {
			onMount(() => {
				const styleEl = document.createElement("style");
				styleEl.textContent = css;
				context.element.renderRoot.appendChild(styleEl);
			});
			return Component(props, context);
		};
	};
};
