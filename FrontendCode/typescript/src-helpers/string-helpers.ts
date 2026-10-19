export const templateReplace = (template: string, values: Record<string, string>) => {
	let tpl = template;
	for (const [key, value] of Object.entries(values)) {
		tpl = tpl.replaceAll(key, value);
	}
	return tpl;
};

export const trim = (str: string, char: string) => {
	let start = 0;
	let end = str.length;

	while (start < end && str[start] === char) {
		start++;
	}

	while (end > start && str[end - 1] === char) {
		end--;
	}

	return start > 0 || end < str.length ? str.slice(start, end) : str;
};
