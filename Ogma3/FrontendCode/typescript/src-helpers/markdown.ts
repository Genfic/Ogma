import { HtmlSanitizer } from "@atulin/html-sanitizer";
import { createInlineFormatExtension } from "@h/markdown-plugins/inline-format-plugin";
import { createMentionExtension } from "@h/markdown-plugins/mention-plugin";
import { marked } from "marked";

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


const sanitizer = new HtmlSanitizer();

// `~~strike~~`, `++insert++` and `==mark==` are emitted by the extensions below but are absent
// from the library's default tag list.
sanitizer.tagWhitelist.DEL = true;
sanitizer.tagWhitelist.INS = true;
sanitizer.tagWhitelist.MARK = true;

// Images need `alt`; the spoiler, mention and hashtag extensions all need `class`.
sanitizer.attributeWhitelist.alt = true;
sanitizer.attributeWhitelist.class = true;

// Nothing this renderer produces uses inline `style` or `id`, and both are risk vectors
// (`style` for overlay CSS, `id` for DOM clobbering).
delete sanitizer.attributeWhitelist.style;
delete sanitizer.attributeWhitelist.id;

// Only schemes a Markdown link or image can legitimately need. This is what rejects
// `javascript:`, `data:`, `file:` and friends. Relative hrefs such as `/user/name` carry no
// scheme and are unaffected.
sanitizer.schemaWhiteList = ["http:", "https:", "mailto:"];

// The library only scheme-checks `href` and `action` (see `uriAttributes` in
// `HtmlSanitizer.ts`), so `![x](javascript:...)` would otherwise pass unchecked.
(
	sanitizer as unknown as {
		uriAttributes: Record<string, boolean>;
	}
).uriAttributes.src = true;

marked.use({
	gfm: true,
	tokenizer: {
		// we're replacing it with a custom implementation
		// emStrong() {
		// 	return undefined;
		// },
	},
	extensions: [
		// createInlineFormatExtension("bold", "*", "strong"),
		// createInlineFormatExtension("italic", "_", "em"),
		// createInlineFormatExtension("super", "^", "sup"),
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
			// Headings render as a `span` to match the server-side `MarkdownPipelines.Comment`
			// `.DisableHeadings()` behaviour.
			return `<span>${this.parser.parseInline(token.tokens)}</span>`;
		},
	},
});

/**
 * Renders Markdown to HTML that is safe to assign to `innerHTML`.
 * @param text raw Markdown, as stored
 * @returns sanitized HTML
 */
export const renderMarkdown = (text: string): string => sanitizer.SanitizeHtml(marked.parse(text, { async: false }));
