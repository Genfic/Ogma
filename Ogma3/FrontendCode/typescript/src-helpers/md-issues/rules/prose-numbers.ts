import { NUMBER_CARDINALS, NUMBER_ORDINALS, isMeasurementUnit, isNonQuantityLabel } from "../numbers";
import type { Issue, IssueRule, MarkdownScan } from "../types";

/** A small cardinal, optionally carrying an English ordinal suffix. */
const SMALL_NUMBER = /\d{1,2}(?:st|nd|rd|th)?/g;

/** The letters immediately before `from`, ignoring any whitespace between them. */
const precedingWord = (text: string, from: number): string => {
	let end = from;
	while (end > 0 && /\s/.test(text[end - 1])) end--;
	let start = end;
	while (start > 0 && /[A-Za-z]/.test(text[start - 1])) start--;
	return text.slice(start, end);
};

/** The letters from `from` up to the next non-letter. */
const followingWord = (text: string, from: number): string => {
	let end = from;
	while (end < text.length && /[A-Za-z]/.test(text[end])) end++;
	return text.slice(from, end);
};

const detect = (scan: MarkdownScan): Issue[] => {
	const { text, prose } = scan;
	const issues: Issue[] = [];

	for (const match of text.matchAll(SMALL_NUMBER)) {
		const start = match.index;
		const end = start + match[0].length;
		if (prose[start] !== 1) continue;

		const value = Number(match[0].replace(/(?:st|nd|rd|th)$/, ""));
		if (value > 20) continue;

		const before = text[start - 1];
		const after = text[end];

		// Part of a longer token, a currency amount, a percentage or a glued unit.
		if (before !== undefined && /[\p{L}\p{N}$£€¥%°]/u.test(before)) continue;
		if (after !== undefined && /[\p{L}\p{N}%°]/u.test(after)) continue;

		// Dates, ranges, versions and decimals keep their digits.
		if (before === "." || before === ":" || before === "-") continue;
		if (after === "." || after === ":" || after === "-") continue;

		// A label such as `chapter 3` is a reference rather than a quantity.
		if (isNonQuantityLabel(precedingWord(text, start))) continue;

		// A measurement such as `5 kg` is clearer with its digits.
		const unitFrom = after === " " ? end + 1 : end;
		if (isMeasurementUnit(followingWord(text, unitFrom))) continue;

		const ordinal = /(?:st|nd|rd|th)$/.test(match[0]);
		const replacement = ordinal ? NUMBER_ORDINALS[value] : NUMBER_CARDINALS[value];

		issues.push({
			rule: "prose-numbers",
			message: `"${match[0]}" is spelled out as "${replacement}"`,
			start,
			end,
			replacement,
		});
	}

	return issues;
};

export const proseNumbers: IssueRule = {
	id: "prose-numbers",
	title: "Spelled-out numbers",
	description:
		"Numbers up to twenty are written as words. Dates, versions, ranges, labels and figures with units keep their digits.",
	detect,
};
