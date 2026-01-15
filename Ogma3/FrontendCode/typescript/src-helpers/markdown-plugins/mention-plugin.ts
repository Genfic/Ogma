export const createMentionExtension = (char: string, urlTemplate: string, className: string) => {
	const escapedChar = char.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");

	// Matches the char followed by alphanumeric, underscores, hyphens, dots, or slashes
	const rule = new RegExp(`^${escapedChar}([a-zA-Z0-9_/.-]+)`);

	return {
		name: className,
		level: "inline" as const,
		start(src: string) {
			return src.indexOf(char);
		},
		tokenizer(src: string) {
			const match = rule.exec(src);
			if (match) {
				return {
					type: className,
					raw: match[0],
					value: match[1],
				};
			}
		},
		renderer(token: { type: string; raw: string; value: string }) {
			const href = urlTemplate.replace("{}", token.value);
			return `<a href="${href}" class="${className}">${char}${token.value}</a>`;
		},
	};
};
