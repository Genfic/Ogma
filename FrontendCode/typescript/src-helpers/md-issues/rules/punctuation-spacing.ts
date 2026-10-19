import type { Issue, IssueRule, MarkdownScan } from "../types";

/** Marks that may sit flush against the punctuation in front of them. */
const HUGGED = new Set([")", "]", "}", "*", "_", "~"]);

/** Stops that take no leading space and always a trailing one. */
const NEEDS_SPACE_AFTER = new Set([".", ",", ";", ":", "!", "?"]);

/** Separators that are numeric when digits sit on both sides. */
const SEPARATORS = new Set([".", ",", ":"]);

/** Number of digits in the run that ends at `at`, stepping by `step`. */
const digitRun = (text: string, at: number, step: number): number => {
	let count = 0;
	for (let i = at; i >= 0 && i < text.length; i += step) {
		if (!/\d/.test(text[i])) {
			break;
		}
		count++;
	}
	return count;
};

/** Whether `index` is a decimal point, digit-group comma or clock colon such as `3.14` or `10:30`. */
const isNumericSeparator = (scan: MarkdownScan, index: number): boolean =>
	SEPARATORS.has(scan.text[index]) && digitRun(scan.text, index - 1, -1) > 0 && digitRun(scan.text, index + 1, 1) > 0;

const detect = (scan: MarkdownScan): Issue[] => {
	const { text, prose } = scan;
	const issues: Issue[] = [];

	for (let i = 0; i < text.length; i++) {
		const ch = text[i];

		if ((ch === " " || ch === "\t") && NEEDS_SPACE_AFTER.has(text[i + 1] ?? "")) {
			const stop = i + 1;
			if (!prose[stop]) {
				continue;
			}

			let start = stop;
			while (start > 0 && (text[start - 1] === " " || text[start - 1] === "\t")) {
				start--;
			}

			const before = text[start - 1];
			// Nothing to close up against, so the stop simply opens a line.
			if (before === undefined || /\s/.test(before)) {
				continue;
			}
			// `3 . 14` is deliberate spacing around a number.
			if (/\d/.test(before) && /\d/.test(text[stop + 1] ?? "")) {
				continue;
			}

			issues.push({
				rule: "punctuation-spacing",
				message: `Space before "${text[stop]}"`,
				start,
				end: stop + 1,
				replacement: text[stop],
			});

			i = stop;
			continue;
		}

		if (!NEEDS_SPACE_AFTER.has(ch)) {
			continue;
		}

		const prev = text[i - 1];
		const next = text[i + 1];

		if (prev === undefined || !/\S/.test(prev)) {
			continue;
		}
		// Already followed by a space or a line break.
		if (next === undefined || /\s/.test(next)) {
			continue;
		}
		if (HUGGED.has(prev)) {
			continue;
		}
		if (!prose[i] || !prose[i - 1]) {
			continue;
		}
		if (isNumericSeparator(scan, i)) {
			continue;
		}

		issues.push({
			rule: "punctuation-spacing",
			message: `Missing space after "${ch}"`,
			start: i + 1,
			end: i + 1,
			replacement: " ",
		});
	}

	return issues;
};

export const punctuationSpacing: IssueRule = {
	id: "punctuation-spacing",
	title: "Punctuation spacing",
	description:
		"Punctuation takes no leading space and always a trailing one, except in decimals, digit groups and times.",
	detect,
};
