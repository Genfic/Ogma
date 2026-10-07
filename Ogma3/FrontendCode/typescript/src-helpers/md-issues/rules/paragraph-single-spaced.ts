import { isSetextUnderline } from "../scan";
import type { Issue, IssueRule, Line, MarkdownScan } from "../types";

/**
 * Characters that may open an inline span, so a line ending in one of them must keep its
 * own line rather than being folded into the next.
 */
const OPEN_DELIMITERS = new Set(["`", "*", "_", "~", "+", "=", "|", "[", "(", '"', "'"]);

/** Whether the line ends in a hard break, either two trailing spaces or a trailing backslash. */
const isHardBreak = (text: string, line: Line): boolean => {
	let content = line.end;
	while (content > line.start && (text[content - 1] === " " || text[content - 1] === "\t")) content--;

	if (line.end - content >= 2) return true;
	return content > line.start && text[content - 1] === "\\";
};

/** Last non-blank character of a line, ignoring trailing whitespace. */
const lastChar = (text: string, line: Line): string | undefined => {
	let end = line.end;
	while (end > line.start && /[ \t]/.test(text[end - 1])) end--;
	return text[end - 1];
};

const detect = (scan: MarkdownScan): Issue[] => {
	const { text } = scan;
	const issues: Issue[] = [];

	for (const paragraph of scan.paragraphs) {
		for (let i = 1; i < paragraph.lineIndexes.length; i++) {
			const previous = scan.lines[paragraph.lineIndexes[i - 1]];
			const current = scan.lines[paragraph.lineIndexes[i]];
			if (previous === undefined || current === undefined) continue;
			// Quotes keep their per-line markers, so only bare prose is folded together.
			if (previous.container !== "prose" || current.container !== "prose") continue;
			if (isHardBreak(text, previous)) continue;
			if (OPEN_DELIMITERS.has(lastChar(text, previous) ?? "")) continue;
			if (isSetextUnderline(text.slice(current.start, current.end))) continue;

			issues.push({
				rule: "paragraph-single-spaced",
				message: "Paragraph is wrapped across lines",
				start: previous.end,
				end: current.contentStart,
				replacement: " ",
			});
		}
	}

	return issues;
};

export const paragraphSingleSpaced: IssueRule = {
	id: "paragraph-single-spaced",
	title: "Wrapped paragraphs",
	description:
		"A paragraph is written on one line. Hard breaks, setext underlines and lines that open an inline span are left alone.",
	detect,
};
