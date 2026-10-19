import type { Component } from "solid-js";

interface CommentProps {
	text: string;
}

/**
 * A SolidJS component that renders a real HTML comment node in the DOM.
 *
 * @param props - The component props, including the 'text' for the comment.
 * @returns A Comment DOM node.
 */
export const Comment: Component<CommentProps> = ({ text }) => {
	return document.createComment(text);
};
