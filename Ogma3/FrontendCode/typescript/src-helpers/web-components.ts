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
	optionalProps?: (keyof T)[],
) => {
	let validated: FunctionComponent<T>;

	if (import.meta.env.DEV) {
		validated = (props, options) => {
			const dbgId = `${createUniqueId()}-${Math.random().toString(36).slice(2, 10)}`;

			const el = options.element;
			el.setAttribute("dbg-id", dbgId);

			const notLowercase: string[] = [];
			for (const attr of el.attributes) {
				if (attr.name !== attr.name.toLowerCase()) {
					notLowercase.push(attr.name);
				}
			}
			if (notLowercase.length > 0) {
				console.error(`<${name}> has invalid HTML attribute(s): ${notLowercase.join(", ")}`, el);
				el.style.outline = "5px solid orange";
				el.style.outlineOffset = "2px";
				setTimeout(() => (el.style.outline = ""), 3000);
			}

			const missing: string[] = [];
			for (const key of Object.keys(defaultProps) as (keyof T)[]) {
				if (optionalProps?.includes(key)) continue;

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

	const comp = styles && styles.length > 0 ? Styled(validated, ...styles) : validated;

	customElement(name, defaultProps, comp);
};
