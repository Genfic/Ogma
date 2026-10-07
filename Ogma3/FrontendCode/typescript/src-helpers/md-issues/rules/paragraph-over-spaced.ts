import type { Issue, IssueRule, MarkdownScan } from "../types";

/** One or more blank lines between blocks, matched across the whole document. */
const BLANK_GAP = /\n[ \t]*\n[ \t]*\n+/g;

/** Number of blank lines in a run of newlines. */
const countBlanks = (gap: string): number => gap.length - gap.replace(/[^\n]/g, "").length - 1;

/**
 * Whether a block starts inside the gap, which proves the gap separates blocks.
 *
 * Fenced code and table rows belong to a single block, so a run of newlines inside one of
 * those is content rather than a separator.
 */
const isBlockBoundary = (scan: MarkdownScan, start: number, end: number): boolean =>
	scan.blocks.some((block) => block.start > start && block.start <= end);

const detect = (scan: MarkdownScan): Issue[] => {
	const issues: Issue[] = [];

	for (const match of scan.text.matchAll(BLANK_GAP)) {
		const start = match.index;
		const end = start + match[0].length;
		if (!isBlockBoundary(scan, start, end)) continue;

		issues.push({
			rule: "paragraph-over-spaced",
			message: `${countBlanks(match[0])} blank lines between blocks`,
			start,
			end,
			replacement: "\n\n",
		});
	}

	return issues;
};

export const paragraphOverSpaced: IssueRule = {
	id: "paragraph-over-spaced",
	title: "Blank lines between blocks",
	description: "Blocks are separated by a single blank line, so two or more blank lines are collapsed to one.",
	detect,
};
