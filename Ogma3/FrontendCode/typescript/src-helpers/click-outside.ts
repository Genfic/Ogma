import type { ICustomElement } from "component-register/types/utils";
import { onCleanup, onMount } from "solid-js";

export const useClickOutside = (element: ICustomElement, callback: () => void) => {
	const handleClick = (event: MouseEvent) => {
		const root = element.renderRoot;
		const path = event.composedPath();
		if (root && !path.includes(root as EventTarget)) {
			callback();
		}
	};
	onMount(() => {
		document.addEventListener("click", handleClick);
	});
	onCleanup(() => {
		document.removeEventListener("click", handleClick);
	});
};

export const onClickOutside = (element: Element, callback: () => void) => {
	const handleClick = (event: MouseEvent) => {
		if (event.composedPath().includes(element)) {
			return;
		}
		callback();
	};
	document.addEventListener("click", handleClick);

	return () => document.removeEventListener("click", handleClick);
};
