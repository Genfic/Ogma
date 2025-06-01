import { HtmlSanitizer } from "@atulin/html-sanitizer";
import { micromark } from "micromark";
import { gfmAutolinkLiteral, gfmAutolinkLiteralHtml } from "micromark-extension-gfm-autolink-literal";
import { gfmStrikethrough, gfmStrikethroughHtml } from "micromark-extension-gfm-strikethrough";
import type { Component } from "solid-js";

const disableExtension = { disable: { null: ["headingAtx"] } };
const sanitizer = new HtmlSanitizer();

export const Markdown: Component<{ text: string }> = (props) => {
	const html = micromark(props.text, {
		extensions: [disableExtension, gfmStrikethrough(), gfmAutolinkLiteral()],
		htmlExtensions: [gfmStrikethroughHtml(), gfmAutolinkLiteralHtml()],
	});
	const sanitized = sanitizer.SanitizeHtml(html);
	return <div class="markdown wc" innerHTML={sanitized} />;
};
