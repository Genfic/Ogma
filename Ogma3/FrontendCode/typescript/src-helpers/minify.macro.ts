export const minifyHtml = (input: string): string => {
	return input
		.split("\n")
		.map((s) => s.trim().replaceAll(/<\s+/g, "<").replaceAll(/\s+>/g, ">"))
		.join("")
		.trim();
};
