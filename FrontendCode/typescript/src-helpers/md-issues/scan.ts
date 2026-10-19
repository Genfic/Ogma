import type { Block, BlockKind, Line, MarkdownScan, Paragraph } from "./types";

const ATX = /^ {0,3}#{1,6}(?:[ \t]|$)/;
const QUOTE = /^ {0,3}> ?/;
const BULLET = /^ {0,3}[-+*](?:[ \t]|$)/;
const ORDERED = /^ {0,3}\d{1,9}[.)](?:[ \t]|$)/;
const HR = /^ {0,3}(?:(?:-|\*|_)[ \t]*){3,}$/;
const FENCE = /^ {0,3}(`{3,}|~{3,})(.*)$/;
const TABLE_DIVIDER = /^[ \t]*\|?[ \t]*:?-+:?[ \t]*(?:\|[ \t]*:?-+:?[ \t]*)*\|?[ \t]*$/;
const HTML_OPEN = /^ {0,3}<(?:!--|\/?[a-zA-Z][a-zA-Z0-9-]*)/;
const SETEXT = /^ {0,3}(?:=+|-+)[ \t]*$/;

/** Delimiter runs produced by the inline-format and emphasis plugins. */
const INLINE_DELIMITERS = new Set(["~", "+", "=", "|", "*", "_"]);

/** Characters trimmed off the end of a bare URL before it counts as URL syntax. */
const URL_TRAILERS = new Set([".", ",", ";", ":", "!", "?"]);

const isSpace = (ch: string | undefined): boolean => ch !== undefined && /\s/.test(ch);
const isWord = (ch: string | undefined): boolean => ch !== undefined && /[A-Za-z0-9_]/.test(ch);

/** Offset of the first non-whitespace character at or after `from`. */
const skipSpace = (text: string, from: number, to: number): number => {
	let i = from;
	while (i < to && isSpace(text[i])) {
		i++;
	}
	return i;
};

/** Blocks whose text is editable prose rather than syntax. */
const isProseContainer = (kind: BlockKind): boolean => kind === "prose" || kind === "quote";

/** Split the source into line ranges, excluding the `\n` terminators. */
const lineRanges = (text: string): { start: number; end: number }[] => {
	const ranges: { start: number; end: number }[] = [];
	let i = 0;

	while (i < text.length) {
		const newline = text.indexOf("\n", i);
		const end = newline === -1 ? text.length : newline;
		ranges.push({ start: i, end });
		if (newline === -1) {
			break;
		}
		i = newline + 1;
	}

	return ranges;
};

/** Length of the block marker that precedes a line's content. */
const markerLength = (kind: BlockKind, line: string): number => {
	switch (kind) {
		case "heading":
			return /^ {0,3}(?:#{1,6})(?:[ \t]+|$)/.exec(line)?.[0].length ?? 0;
		case "quote":
			return QUOTE.exec(line)?.[0].length ?? 0;
		case "list":
			return /^ {0,3}(?:[-+*]|\d{1,9}[.)])(?:[ \t]+|$)/.exec(line)?.[0].length ?? 0;
		case "html":
			return /^ {0,3}</.exec(line)?.[0].length ?? 0;
		default:
			return 0;
	}
};

/** Whether an open fence is terminated by `line`. */
const closesFence = (line: string, fence: string): boolean =>
	new RegExp(`^ {0,3}${fence[0]}{${fence.length},}[ \\t]*$`).test(line);

/** Scan a Markdown document once, producing everything the rules need. */
export const scanMarkdown = (text: string): MarkdownScan => {
	const ranges = lineRanges(text);
	const containers: BlockKind[] = ranges.map(({ start, end }) => classify(text.slice(start, end)));

	markTables(ranges, containers, text);
	markFences(ranges, containers, text);

	const lines: Line[] = ranges.map(({ start, end }, index) => {
		const container = containers[index];
		const contentStart = skipSpace(text, start + markerLength(container, text.slice(start, end)), end);
		return { start, end, blank: contentStart >= end, container, contentStart };
	});

	const prose = new Uint8Array(text.length);
	for (const line of lines) {
		if (!isProseContainer(line.container)) {
			continue;
		}
		for (let p = line.contentStart; p < line.end; p++) {
			prose[p] = 1;
		}
		maskInline(text, line.contentStart, line.end, prose);
	}

	const blocks: Block[] = [];
	let broken = false;

	for (const [index, line] of lines.entries()) {
		// Blank lines inside a fence belong to its block, anywhere else they separate blocks.
		if (line.blank) {
			if (line.container !== "code") {
				broken = true;
			}
			continue;
		}

		const prev = blocks.at(-1);
		if (!broken && prev !== undefined && prev.kind === line.container) {
			prev.lastLine = index;
			prev.end = line.end;
		} else {
			blocks.push({
				kind: line.container,
				start: line.start,
				end: line.end,
				firstLine: index,
				lastLine: index,
			});
		}

		broken = false;
	}

	const paragraphs: Paragraph[] = [];
	let current: Paragraph | undefined;
	for (const [index, line] of lines.entries()) {
		if (!isProseContainer(line.container) || line.blank) {
			current = undefined;
			continue;
		}

		if (current === undefined) {
			current = { start: line.contentStart, end: line.end, lineIndexes: [] };
			paragraphs.push(current);
		}

		current.lineIndexes.push(index);
		current.end = line.end;
	}

	return { text, prose, lines, blocks, paragraphs };
};

/** Assign a container to a line that is not inside a fence or an HTML block. */
const classify = (line: string): BlockKind => {
	if (ATX.test(line)) {
		return "heading";
	}
	if (HR.test(line)) {
		return "html";
	}
	if (FENCE.test(line)) {
		return "code";
	}
	if (QUOTE.test(line)) {
		return "quote";
	}
	if (BULLET.test(line) || ORDERED.test(line)) {
		return "list";
	}
	if (HTML_OPEN.test(line)) {
		return "html";
	}
	if (TABLE_DIVIDER.test(line)) {
		return "table";
	}

	return "prose";
};

/** Promote a pipe-delimited run of lines to `table`, anchored on the divider row. */
const markTables = (ranges: { start: number; end: number }[], containers: BlockKind[], text: string): void => {
	for (let index = 0; index < ranges.length; index++) {
		if (containers[index] !== "prose") {
			continue;
		}

		const line = text.slice(ranges[index].start, ranges[index].end);
		const next = ranges[index + 1];
		if (next === undefined || !line.includes("|")) {
			continue;
		}
		if (!TABLE_DIVIDER.test(text.slice(next.start, next.end))) {
			continue;
		}

		for (let scan = index; scan < ranges.length; scan++) {
			if (containers[scan] !== "prose" && containers[scan] !== "table") {
				break;
			}
			if (!text.slice(ranges[scan].start, ranges[scan].end).includes("|")) {
				break;
			}
			containers[scan] = "table";
		}
	}
};

/** Rewrite the fence delimiters and everything they wrap to `code`. */
const markFences = (ranges: { start: number; end: number }[], containers: BlockKind[], text: string): void => {
	let fence: string | null = null;

	for (const [index, { start, end }] of ranges.entries()) {
		const line = text.slice(start, end);

		if (fence !== null) {
			containers[index] = "code";
			if (closesFence(line, fence)) {
				fence = null;
			}
			continue;
		}

		if (containers[index] !== "code") {
			continue;
		}
		fence = FENCE.exec(line)?.[1] ?? null;
	}
};

/** Mask out inline syntax inside `[from, to)` so rules never rewrite delimiters or URLs. */
const maskInline = (text: string, from: number, to: number, prose: Uint8Array): void => {
	const block = (start: number, end: number) => {
		const hi = Math.min(end, to);
		for (let p = start; p < hi; p++) {
			prose[p] = 0;
		}
	};

	/** Index of the `close` matching the `open` at `at`, honouring nesting and escapes. */
	const matchPair = (open: string, close: string, at: number): number => {
		let depth = 0;
		for (let p = at; p < to; p++) {
			if (text[p] === "\\") {
				p++;
				continue;
			}
			if (text[p] === open) {
				depth++;
			} else if (text[p] === close && --depth === 0) {
				return p;
			}
		}
		return -1;
	};

	/** Mask a code span opened by a backtick run at `at`. Returns the offset to resume from. */
	const maskCodeSpan = (at: number): number => {
		let run = 1;
		while (at + run < to && text[at + run] === "`") {
			run++;
		}

		let cursor = at + run;
		while (cursor < to) {
			const open = text.indexOf("`", cursor);
			if (open === -1 || open >= to) {
				break;
			}

			let closeRun = 1;
			while (open + closeRun < to && text[open + closeRun] === "`") {
				closeRun++;
			}

			if (closeRun === run) {
				block(at, open + closeRun);
				return open + closeRun;
			}

			cursor = open + closeRun;
		}

		block(at, at + run);
		return at + run;
	};

	let i = from;
	while (i < to) {
		const ch = text[i];

		if (ch === "\\") {
			block(i, i + 2);
			i += 2;
			continue;
		}

		if (ch === "`") {
			i = maskCodeSpan(i);
			continue;
		}

		if (ch === "&") {
			const entity = /^&(?:#\d+|#x[0-9a-fA-F]+|[a-zA-Z][a-zA-Z0-9]*);/.exec(text.slice(i, to));
			if (entity) {
				block(i, i + entity[0].length);
				i += entity[0].length;
				continue;
			}
		}

		if (ch === "<") {
			if (text.startsWith("<!--", i)) {
				const close = text.indexOf("-->", i + 4);
				const stop = close === -1 ? to : close + 3;
				block(i, stop);
				i = stop;
				continue;
			}

			const close = text.indexOf(">", i + 1);
			if (close !== -1 && close < to) {
				const inner = text.slice(i + 1, close);
				const autolink =
					/^[a-zA-Z][a-zA-Z0-9+.-]*:\S*$/.test(inner) || /^[^\s<>@]+@[^\s<>@]+\.\S+$/.test(inner);
				if (autolink || /^!?\/?[a-zA-Z][a-zA-Z0-9-]*(?:\s[^<>]*)?$/.test(inner)) {
					block(i, close + 1);
					i = close + 1;
					continue;
				}
			}
		}

		if (ch === "[") {
			const close = matchPair("[", "]", i);
			if (close === -1) {
				i++;
				continue;
			}

			block(i, i + 1);

			if (text[close + 1] === "(") {
				const dest = matchPair("(", ")", close + 1);
				if (dest !== -1) {
					block(close, dest + 1);
					i = dest + 1;
					continue;
				}
			}

			if (text[close + 1] === "[") {
				const ref = matchPair("[", "]", close + 1);
				if (ref !== -1) {
					block(close, ref + 1);
					i = ref + 1;
					continue;
				}
			}

			block(close, close + 1);
			i = close + 1;
			continue;
		}

		if (ch === "@" || ch === "#") {
			let handle = i + 1;
			while (handle < to && /[A-Za-z0-9_./-]/.test(text[handle])) {
				handle++;
			}
			if (handle > i + 1) {
				block(i, handle);
				i = handle;
				continue;
			}
		}

		if (!isWord(text[i - 1])) {
			const url =
				(ch === "h" && (text.startsWith("http://", i) || text.startsWith("https://", i))) ||
				(ch === "w" && text.startsWith("www.", i));

			if (url) {
				let end = i;
				while (end < to && !isSpace(text[end]) && text[end] !== "<") {
					end++;
				}
				while (end > i && URL_TRAILERS.has(text[end - 1])) {
					end--;
				}
				block(i, end);
				i = end;
				continue;
			}
		}

		if (INLINE_DELIMITERS.has(ch)) {
			let run = 1;
			while (i + run < to && text[i + run] === ch) {
				run++;
			}
			const width = ch === "~" || ch === "=" ? 1 : run;

			const close = text.indexOf(ch.repeat(width), i + width);
			if (close !== -1 && close < to && !isSpace(text[close - 1])) {
				block(i, i + width);
				block(close, close + width);
				i = close + width;
				continue;
			}

			i += run;
			continue;
		}

		i++;
	}
};

/** Whether a line is a setext underline, which must not be folded into its heading. */
export const isSetextUnderline = (line: string): boolean => SETEXT.test(line);
