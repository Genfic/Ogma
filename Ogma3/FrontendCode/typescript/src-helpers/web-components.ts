import type { FunctionComponent } from "component-register";
import { customElement } from "solid-element";
import { createUniqueId } from "solid-js";
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
	let validated: FunctionComponent<T>;

	if (import.meta.env.DEV) {
		const dbgId = `${createUniqueId()}-${Math.random().toString(36).slice(2, 10)}`;

		validated = (props, options) => {
			const el = options.element;
			el.setAttribute("dbg-id", dbgId);

			const missing: string[] = [];
			for (const key of Object.keys(defaultProps) as (keyof T)[]) {
				const value = props[key];
				if (
					[undefined, null, "", 0].includes(value) ||
					(Array.isArray(value) && (value as undefined[]).length === 0) ||
					(typeof value === "number" && Number.isNaN(value))
				) {
					missing.push(String(key));
				}
			}
			if (missing.length > 0) {
				console.error(`<${name}> is missing required props: ${missing.join(", ")}`, el);
				el.style.outline = "5px solid red";
				el.style.outlineOffset = "2px";
				setTimeout(() => (el.style.outline = ""), 3000);
			}

			return component(props, options);
		};
	} else {
		validated = component;
	}

	const comp = styles ? Styled(validated, ...styles) : validated;

	customElement(name, defaultProps, comp);
};
