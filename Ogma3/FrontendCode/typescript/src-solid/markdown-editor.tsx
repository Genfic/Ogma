import { type ComponentType, customElement } from "solid-element";
import { createEffect, For, type JSX, onCleanup, onMount } from "solid-js";
import css from "./markdown-editor.css";
import sharedCss from "./shared.css";
import { Styled } from "./common/_styled";
import { Comment } from "./common/_comment";
import { createHistory } from "solid-signals";

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
	typeof node === "object" &&
	node !== null &&
	"nodeName" in node &&
	(node.nodeName === "TEXTAREA" || node.nodeName === "INPUT");

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

	const [cursorPosition, setCursorPosition] = createHistory(0);
	const [content, setContent] = createHistory(area.value);

	createEffect(() => {
		area.value = content();
	});

	const handleCtrlZ = (e: KeyboardEvent) => {
		if (content.history().length <= 1) {
			return;
		}
		if (e.ctrlKey && e.key === "z") {
			setContent.history.back();
			const cursor = cursorPosition.history().at(-1) ?? 0;
			area.setSelectionRange(cursor, cursor);
			setCursorPosition.history.back();
			return;
		}
	};

	onMount(() => {
		area.addEventListener("keydown", handleCtrlZ);
	});
	onCleanup(() => {
		area.removeEventListener("keydown", handleCtrlZ);
	});

	const click = ({ prefix, suffix }: Action) => {
		const start = area.selectionStart ?? 0;
		const end = area.selectionEnd ?? 0;
		const text = area.value.substring(start, end);
		const newText = `${prefix}${text}${suffix}`;

		area.setRangeText(newText, start, end, "preserve");
		area.selectionStart = start + prefix.length;
		area.selectionEnd = end + prefix.length;

		setContent(area.value);
		setCursorPosition(end);

		area.focus();
		area.dispatchEvent(new Event("input"));
	};

	return (
		<nav class="toolbar">
			<Comment text={selectorActual} />
			<For each={actions}>
				{(action) => (
					<button type="button" class="btn action-btn" title={action.name} onClick={[click, action]}>
						<o-icon icon={action.icon} class="icon" />
					</button>
				)}
			</For>
		</nav>
	);
};

customElement(name, { selector: "textarea", overrideSelector: false }, Styled(MarkdownEditor, sharedCss, css));
