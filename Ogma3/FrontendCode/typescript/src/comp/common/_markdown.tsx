import { renderMarkdown } from "@h/markdown";
import type { Component } from "solid-js";

export const Markdown: Component<{ text: string; class?: string }> = (props) => (
	<div class={`${props.class} markdown md`} innerHTML={renderMarkdown(props.text)} />
);
