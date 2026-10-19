/**
 * Contracts for the deterministic Markdown writing-issue detector.
 *
 * Every rule works on a single {@link MarkdownScan} and reports byte-accurate
 * source ranges, so edits can be applied without re-searching the text.
 */

/** Coarse block classification of a source line. */
export type BlockKind = "prose" | "quote" | "list" | "heading" | "code" | "table" | "html";

/** Stable identifier of a rule, used for grouping and for {@link Issue.rule}. */
export type RuleId =
	| "sentence-capital"
	| "punctuation-spacing"
	| "paragraph-single-spaced"
	| "paragraph-over-spaced"
	| "prose-numbers";

/** A single actionable problem in the source text. */
export type Issue = {
	rule: RuleId;
	/** Short human readable explanation, e.g. `Sentence starts with a lowercase letter`. */
	message: string;
	/** Inclusive start offset into the source text. */
	start: number;
	/** Exclusive end offset into the source text. */
	end: number;
	/** Text that replaces `[start, end)`. Equal to the original slice when nothing needs changing. */
	replacement: string;
};

/** One rule, ready to run against a scan. */
export type IssueRule = {
	id: RuleId;
	title: string;
	description: string;
	detect: (scan: MarkdownScan) => Issue[];
};

/** A single source line. `contentStart` points just past the block marker (`# `, `- `, `> `). */
export type Line = {
	/** Offset of the first character of the line. */
	start: number;
	/** Offset just past the last character of the line, excluding the line terminator. */
	end: number;
	/** True when the line holds no content beyond its block marker. */
	blank: boolean;
	/** Block classification of the line. */
	container: BlockKind;
	/** Offset of the first non-whitespace character after any block marker. */
	contentStart: number;
};

/** A run of consecutive lines sharing a single {@link BlockKind}. */
export type Block = {
	kind: BlockKind;
	start: number;
	end: number;
	/** Inclusive index of the first line in {@link MarkdownScan.lines}. */
	firstLine: number;
	/** Inclusive index of the last line in {@link MarkdownScan.lines}. */
	lastLine: number;
};

/** A maximal run of consecutive non-blank prose lines, i.e. one Markdown paragraph. */
export type Paragraph = {
	start: number;
	end: number;
	/** Indices into {@link MarkdownScan.lines}, in order. */
	lineIndexes: number[];
};

/** Result of scanning a Markdown document once. */
export type MarkdownScan = {
	text: string;
	/**
	 * Per-character mask. `1` means the character is editable prose and may take part in an
	 * issue; `0` means it is code, syntax, or a block marker and must never be rewritten.
	 */
	prose: Uint8Array;
	lines: Line[];
	blocks: Block[];
	paragraphs: Paragraph[];
};

/** Rules that rewrite whitespace or punctuation may coalesce their diagnostics. */
export type CoalescingRule = RuleId;
