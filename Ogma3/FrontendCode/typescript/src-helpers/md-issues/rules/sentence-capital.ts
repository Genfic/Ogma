import { findSentenceStarts, needsCapital } from "../sentences";
import type { Issue, IssueRule, MarkdownScan } from "../types";

const detect = (scan: MarkdownScan): Issue[] => {
	const { text, prose } = scan;
	const issues: Issue[] = [];

	for (const paragraph of scan.paragraphs) {
		for (const start of findSentenceStarts(scan, paragraph.start, paragraph.end)) {
			// Openers such as quotes and emphasis marks sit between the start and the word.
			let word = start;
			while (word < paragraph.end && !/[A-Za-z\d]/.test(text[word])) {
				word++;
			}

			// A sentence that opens on a digit has no letter to capitalise.
			if (word >= paragraph.end || !/[A-Za-z]/.test(text[word]) || !prose[word]) {
				continue;
			}

			const letter = text[word];
			const lower = letter.toLowerCase();
			if (letter !== lower) {
				continue;
			}

			let end = word;
			while (end < paragraph.end && /[A-Za-z']/.test(text[end])) {
				end++;
			}

			if (!needsCapital(text.slice(word, end))) {
				continue;
			}

			issues.push({
				rule: "sentence-capital",
				message: `Sentence starts with the lowercase letter "${lower}"`,
				start: word,
				end: word + 1,
				replacement: lower.toUpperCase(),
			});
		}
	}

	return issues;
};

export const sentenceCapital: IssueRule = {
	id: "sentence-capital",
	title: "Sentence capitals",
	description:
		"Sentence openings must start with a capital letter. Initialisms, decimals and known abbreviations are left alone.",
	detect,
};
