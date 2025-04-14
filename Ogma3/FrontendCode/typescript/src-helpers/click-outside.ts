import type { ICustomElement } from "component-register/types/utils";

export const clickOutside = (element: HTMLElement, callback: () => void) =>
	document.addEventListener("click", (e) => {
		if (!e.composedPath().includes(element)) {
			callback();
		}
	});

export const clickOutsideSolid = (element: ICustomElement, callback: () => void) =>
	document.addEventListener("click", (e) => {
		if (!e.composedPath().includes(element.renderRoot)) {
			callback();
		}
	});
