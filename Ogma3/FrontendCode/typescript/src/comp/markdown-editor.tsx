import { $query } from "@h/dom";
import { component } from "@h/web-components";
import type { ComponentType } from "solid-element";
import { createEffect, For, onCleanup, onMount } from "solid-js";
import { createHistory } from "solid-signals";
import { LucideBold } from "../icons/LucideBold";
import { LucideEyeClosed } from "../icons/LucideEyeClosed";
import { LucideItalic } from "../icons/LucideItalic";
import { LucideLink } from "../icons/LucideLink";
import { LucideStrikethrough } from "../icons/LucideStrikethrough";
import { Comment } from "./common/_comment";
import type { ExtraButtonContext } from "./extra-buttons/extra-button-types";
import css from "./markdown-editor.css";
import sharedCss from "./shared.css";

const actions = [
	{ name: "bold", icon: () => <LucideBold />, prefix: "**", suffix: "**" },
	{ name: "italic", icon: () => <LucideItalic />, prefix: "*", suffix: "*" },
	{ name: "strikethrough", icon: () => <LucideStrikethrough />, prefix: "~~", suffix: "~~" },
	{ name: "link", icon: () => <LucideLink />, prefix: "[", suffix: "](url)" },
	{ name: "spoiler", icon: () => <LucideEyeClosed />, prefix: "||", suffix: "||" },
];

type Action = (typeof actions)[number];

const name = "markdown-editor" as const;

type Props = {
	selector: `textarea${string | ""}` | `input${string | ""}`;
	overrideSelector?: boolean;
};

export const MarkdownEditor: ComponentType<Props> = ({ selector, overrideSelector }) => {
	const selectorActual = overrideSelector ? selector : `${name} + ${selector}`;
	const area = $query<HTMLTextAreaElement | HTMLInputElement>(selectorActual);

	if (!area) {
		throw Error(`Element "${selector}" not found`);
	}

	if (!["TEXTAREA", "INPUT"].includes(area.nodeName)) {
		throw Error(`Element "${selector}" is not a textarea or input`);
	}

	const slot = $signal<HTMLSlotElement>();

	const onReslotted = async () => {
		for (const el of slot?.assignedElements() ?? []) {
			await customElements.whenDefined(el.localName);

			if (!("context" in el)) {
				console.info("MarkdownEditor: Found element without context attribute", el);
				continue;
			}

			el.context = {
				input: area,
			} satisfies ExtraButtonContext;
		}
	};

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

		if (text.substring(0, prefix.length) === prefix && text.substring(text.length - suffix.length) === suffix) {
			const inner = text.substring(prefix.length, text.length - suffix.length);
			area.setRangeText(inner, start, end, "preserve");
			area.selectionStart = start;
			area.selectionEnd = start + inner.length;
		} else if (
			area.value.substring(start - prefix.length, start) === prefix &&
			area.value.substring(end, end + suffix.length) === suffix
		) {
			area.setRangeText(text, start - prefix.length, end + suffix.length, "preserve");
			area.selectionStart = start - prefix.length;
			area.selectionEnd = end - prefix.length;
		} else {
			area.setRangeText(`${prefix}${text}${suffix}`, start, end, "preserve");
			area.selectionStart = start + prefix.length;
			area.selectionEnd = end + prefix.length;
		}

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
						{action.icon()}
					</button>
				)}
			</For>
			<slot ref={$set(slot)} onslotchange={onReslotted} />
		</nav>
	);
};

component(name, { selector: "textarea", overrideSelector: false }, MarkdownEditor, [sharedCss, css]);
