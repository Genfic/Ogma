export const dedent = (strings: TemplateStringsArray, ...values: unknown[]) => {
	const lines = strings.reduce((acc, str, idx) => acc + str + (values[idx] ?? ""), "").split("\n");

	if (lines.length > 0 && lines[lines.length - 1].trim() === "") {
		lines.pop();
	}

	let commonIndent = Number.POSITIVE_INFINITY;

	for (let i = 0; i < lines.length; i++) {
		const line = lines[i];

		if (line.trim() === "") {
			continue;
		}

		let indent = 0;
		while (indent < line.length && (line[indent] === " " || line[indent] === "\t")) {
			indent++;
		}

		if (indent < commonIndent) {
			commonIndent = indent;
		}
	}

	if (commonIndent === Number.POSITIVE_INFINITY) {
		return "";
	}

	for (let i = 0; i < lines.length; i++) {
		if (lines[i].trim() === "") {
			lines[i] = "";
		} else {
			lines[i] = lines[i].slice(commonIndent);
		}
	}

	return lines.join("\n");
};
