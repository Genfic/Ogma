import type { FunctionComponent } from "component-register";
import { flattenObject } from "es-toolkit";
import { customElement } from "solid-element";
import { Styled } from "../src/comp/common/_styled";

type HTMLAttributeNames = keyof HTMLElement;

type NoHTMLAttributeKeys<T> = {
	[K in keyof T]: K extends HTMLAttributeNames ? `Property '${K & string}' conflicts with HTML attribute` : T[K];
};

export const component = <T extends NoHTMLAttributeKeys<T>>(
	name: `${string}-${string}`,
	defaultProps: T,
	component: FunctionComponent<T>,
	styles?: string[] | string,
) => {
	const comp = styles ? Styled(component, ...styles) : component;

	Object.entries(flattenObject(defaultProps)).forEach(([key, value]) => {
		if (["", 0, false, null, undefined].includes(value) || (Array.isArray(value) && value.length === 0)) {
			console.error(`Property '${key}' of component '${name}' has no value.`);
		}
	});

	customElement(name, defaultProps, comp);
};
