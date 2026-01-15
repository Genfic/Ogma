import { createInlineFormatExtension } from "@h/markdown-plugins/inline-format-plugin";
import { createMentionExtension } from "@h/markdown-plugins/mention-plugin";
import { marked } from "marked";
import type { Component } from "solid-js";

const escapeHTML = (text: string) =>
	text.replace(
		/[&<>"']/g,
		(m) =>
			({
				"&": "&amp;",
				"<": "&lt;",
				">": "&gt;",
				'"': "&quot;",
				"'": "&#39;",
			})[m] || m,
	);

marked.use({
	gfm: true,
	tokenizer: {
		emStrong() {
			return undefined;
		},
	},
	extensions: [
		createInlineFormatExtension("bold", "*", "strong"),
		createInlineFormatExtension("italic", "_", "em"),
		createInlineFormatExtension("super", "^", "sup"),
		createInlineFormatExtension("sub", "~", "sub"),
		createInlineFormatExtension("insert", "++", "ins"),
		createInlineFormatExtension("mark", "==", "mark"),
		createInlineFormatExtension("spoiler", "||", "span", "spoiler"),

		createMentionExtension("@", "/user/{}", "mention"),
		createMentionExtension("#", "/tag/{}", "hashtag"),
	],
	renderer: {
		html(token) {
			return escapeHTML(token.text);
		},
		heading(token) {
			return `<span>${token.raw.trim()}</span>`;
		},
	},
});

export const Markdown: Component<{ text: string; class?: string }> = (props) => {
	const html = marked.parse(props.text, { async: false });
	return <div class={`${props.class} markdown md`} innerHTML={html} />;
};
