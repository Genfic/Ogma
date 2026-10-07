import type { MarkdownScan } from "./types";

/**
 * Words that end in a full stop without ending a sentence. Matched
 * case-insensitively against the letters preceding the stop.
 */
export const ABBREVIATIONS: readonly string[] = [
	"mr",
	"mrs",
	"ms",
	"mx",
	"dr",
	"prof",
	"rev",
	"hon",
	"st",
	"ste",
	"ave",
	"rd",
	"blvd",
	"etc",
	"vs",
	"viz",
	"al",
	"approx",
	"est",
	"dept",
	"univ",
	"vol",
	"pp",
	"fig",
	"sec",
	"cf",
	"ca",
	"circa",
	"min",
	"max",
	"misc",
	"incl",
	"excl",
	"resp",
	"jan",
	"feb",
	"mar",
	"apr",
	"jun",
	"jul",
	"aug",
	"sep",
	"sept",
	"oct",
	"nov",
	"dec",
];

/**
 * Words that cannot open a sentence. Capitalised openers such as "But" are still
 * continuations, because a real sentence start would not be capitalised anyway.
 */
export const CONTINUATION_WORDS: readonly string[] = [
	"and",
	"or",
	"but",
	"nor",
	"yet",
	"so",
	"for",
	"if",
	"then",
	"else",
	"when",
	"while",
	"because",
	"since",
	"unless",
	"although",
	"though",
	"whereas",
	"after",
	"before",
	"until",
	"once",
	"as",
	"that",
	"which",
	"who",
	"whom",
	"whose",
	"i",
	"you",
	"he",
	"she",
	"it",
	"we",
	"they",
	"is",
	"are",
	"was",
	"were",
	"be",
	"been",
	"being",
	"am",
	"do",
	"does",
	"did",
	"have",
	"has",
	"had",
	"will",
	"would",
	"can",
	"could",
	"should",
	"may",
	"might",
	"must",
];

/** Characters that may sit between a sentence start and its first word. */
const OPENERS = "([{\"'“‘*_~";

/** The letters and inner full stops that immediately precede the stop at `index`. */
const precedingToken = (text: string, index: number): string => {
	let start = index;
	while (start > 0 && /[A-Za-z.]/.test(text[start - 1])) start--;
	return text.slice(start, index);
};

/**
 * Whether the full stop at `index` closes a sentence.
 *
 * False for decimals, ellipses, initialisms such as `U.S`, detached periods, and
 * known abbreviations — every case that reads as a mid-sentence stop.
 */
export const isSentenceEnd = (text: string, index: number): boolean => {
	const next = text[index + 1];
	if (next !== undefined && /\d/.test(next)) return false;
	if (next === ".") return false;

	const token = precedingToken(text, index);

	// "U.S." and "a.m." carry an inner full stop before the final one.
	if (token.includes(".")) return false;

	// A single letter is an initial, and no letters at all is a number or marker.
	if (token.length <= 1) return false;

	return !ABBREVIATIONS.includes(token.toLowerCase());
};

/** Whether the word starting at `at` cannot open a sentence. */
const continuesSentence = (text: string, at: number, to: number): boolean => {
	if (at >= to) return true;

	let from = at;
	if (OPENERS.includes(text[from])) from++;
	while (from < to && !/[A-Za-z]/.test(text[from])) from++;

	let end = from;
	while (end < to && /[A-Za-z]/.test(text[end])) end++;

	if (end === from) return false;

	return CONTINUATION_WORDS.includes(text.slice(from, end).toLowerCase());
};

/**
 * Offsets of every sentence start within `[from, to)`, in source order.
 *
 * A sentence starts at the first prose character of a paragraph and after every
 * sentence-ending stop, skipping openers that continue the previous sentence.
 */
export const findSentenceStarts = (scan: MarkdownScan, from: number, to: number): number[] => {
	const { text, prose } = scan;
	const starts: number[] = [];

	const opening = new Uint8Array(text.length);
	for (const paragraph of scan.paragraphs) {
		const line = scan.lines[paragraph.lineIndexes[0]];
		if (line !== undefined) opening[line.contentStart] = 1;
	}

	for (let i = from; i < to; i++) {
		if (!prose[i]) continue;

		if (opening[i] === 1) {
			if (!continuesSentence(text, i, to)) starts.push(i);
			continue;
		}

		const stop = text[i];
		if (stop !== "." && stop !== "!" && stop !== "?") continue;

		if (stop === "." && !isSentenceEnd(text, i)) continue;

		// The next sentence begins at the first character after the stop and any spacing.
		let next = i + 1;
		while (next < to && (text[next] === " " || text[next] === "\t")) next++;
		if (next >= to || !prose[next]) continue;

		// Continue scanning from the word itself, so its letters are not re-examined as stops.
		i = next - 1;

		if (continuesSentence(text, next, to)) continue;

		starts.push(next);
	}

	return starts;
};

/** Whether `word` is allowed to open a sentence, and so should carry a capital. */
export const needsCapital = (word: string): boolean => !CONTINUATION_WORDS.includes(word.toLowerCase());
