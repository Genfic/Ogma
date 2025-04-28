import { type ComponentType, customElement } from "solid-element";
import { For, type JSX } from "solid-js";
import css from "./markdown-editor.css";
import { styled } from "@h/jsx-wc-style";
import { Comment } from "./common/_comment";

type Action = {
	name: string;
	icon: string;
	prefix: string;
	suffix: string;
};

const actions: Action[] = [
	{ name: "bold", icon: "lucide:bold", prefix: "**", suffix: "**" },
	{ name: "italic", icon: "lucide:italic", prefix: "*", suffix: "*" },
	{ name: "strikethrough", icon: "lucide:strikethrough", prefix: "~~", suffix: "~~" },
	{ name: "link", icon: "lucide:link", prefix: "[", suffix: "](url)" },
	{ name: "spoiler", icon: "lucide:eye-closed", prefix: "||", suffix: "||" },
] as const;

const name = "markdown-editor" as const;

const isTextAreaOrInput = (node: JSX.Element): node is HTMLTextAreaElement | HTMLInputElement =>
	typeof node === "object" && "nodeName" in node && (node.nodeName === "TEXTAREA" || node.nodeName === "INPUT");

type Props = {
	selector: `textarea${string | ""}` | `input${string | ""}`;
	overrideSelector?: boolean;
};

export const MarkdownEditor: ComponentType<Props> = ({ selector, overrideSelector }) => {
	const selectorActual = overrideSelector ? selector : `${name} + ${selector}`;
	const area = document.querySelector(selectorActual);

	if (!area) {
		throw Error(`Element "${selector}" not found`);
	}

	if (!isTextAreaOrInput(area)) {
		throw Error(`Element "${selector}" is not a textarea or input`);
	}

	const click = ({ prefix, suffix }: Action) => {
		const start = area.selectionStart;
		const end = area.selectionEnd;
		const text = area.value.substring(start, end);
		const newText = `${prefix}${text}${suffix}`;

		area.setRangeText(newText, start, end, "preserve");
		area.selectionStart = start + prefix.length;
		area.selectionEnd = end + prefix.length;

		area.focus();
		area.dispatchEvent(new Event("input"));
	};

	return (
		<nav class="toolbar">
			<Comment text={selectorActual} />
			<For each={actions}>
				{(action) => (
					<button type="button" class="btn" title={action.name} onClick={[click, action]}>
						<o-icon icon={action.icon} class="material-icons-outlined" />
					</button>
				)}
			</For>
		</nav>
	);
};

customElement<Props>(name, { selector: "textarea", overrideSelector: false }, styled(css)(MarkdownEditor));
