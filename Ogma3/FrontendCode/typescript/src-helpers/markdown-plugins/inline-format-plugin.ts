import type { Renderer, Token, Tokenizer } from "marked";

export const createInlineFormatExtension = (name: string, char: string, htmlTag: string, className?: string) => {
	const escapedChar = char.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");

	// 1. (?!${escapedChar}) -> Not followed by the same char (prevents ~ matching ~~)
	// 2. (?!\s) -> Content cannot start with a space
	// 3. ([\s\S]+?) -> The content
	// 4. (?<!\s) -> Content cannot end with a space
	// 5. (?<!${escapedChar}) -> Not preceded by the same char (prevents ~~ matching ~)
	const rule = new RegExp(
		`^${escapedChar}(?!${escapedChar})(?!\\s)([\\s\\S]+?)(?<!\\s)(?<!${escapedChar})${escapedChar}(?!${escapedChar})`,
	);

	return {
		name,
		level: "inline" as const,
		start(src: string) {
			return src.indexOf(char);
		},
		tokenizer(this: Tokenizer, src: string) {
			const match = rule.exec(src);
			if (match) {
				return {
					type: name,
					raw: match[0],
					tokens: this.lexer.inlineTokens(match[1]),
				};
			}
		},
		renderer(this: Renderer, token: { type: string; raw: string; tokens: Token[] }) {
			const classAttr = className ? ` class="${className}"` : "";
			return `<${htmlTag}${classAttr}>${this.parser.parseInline(token.tokens)}</${htmlTag}>`;
		},
	};
};
