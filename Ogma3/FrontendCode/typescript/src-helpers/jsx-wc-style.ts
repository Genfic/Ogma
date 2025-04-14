import type { ICustomElement } from "component-register/types/utils";

export const addStyle = (element: ICustomElement, style: string) => {
	const styleEl = document.createElement("style");

	styleEl.textContent = style;

	element.shadowRoot.appendChild(styleEl);
};
