export const dedent = (strings: TemplateStringsArray, ...values: unknown[]) => {
	const lines = strings.reduce((acc, str, idx) => acc + str + (values[idx] ?? ""), "").split("\n");

	const indent = lines.reduce(
		(acc, line) => Math.min(acc, line.length - line.trimStart().length),
		Number.POSITIVE_INFINITY,
	);
	return lines
		.map((line) => line.slice(indent))
		.join("\n")
		.trim();
};
