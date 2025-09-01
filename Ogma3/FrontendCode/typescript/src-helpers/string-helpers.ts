export const templateReplace = (template: string, values: Record<string, string>) => {
	let tpl = template;
	for (const [key, value] of Object.entries(values)) {
		tpl = tpl.replaceAll(key, value);
	}
	return tpl;
};
