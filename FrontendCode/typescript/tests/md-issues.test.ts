import { describe, expect, test } from "bun:test";
import { analyze, fixIssues } from "../src-helpers/md-issues";

/** Issues of a single rule, for terse assertions. */
const ofRule = (text: string, rule: string): number => analyze(text).filter((issue) => issue.rule === rule).length;

/** Every reported issue for `text`, as `rule@start:end` strings. */
const report = (text: string): string[] => analyze(text).map((issue) => `${issue.rule}@${issue.start}:${issue.end}`);

describe("sentence-capital", () => {
	test("reports a lowercase paragraph opener", () => {
		expect(ofRule("this is a sentence.", "sentence-capital")).toBe(1);
	});

	test("reports a lowercase word after a sentence end", () => {
		expect(report("One thing. cats are nice.")).toEqual(["sentence-capital@11:12"]);
	});

	test("accepts an already capitalised opener", () => {
		expect(ofRule("This is fine.", "sentence-capital")).toBe(0);
	});

	test("leaves initials alone", () => {
		expect(ofRule("A. Person wrote this.", "sentence-capital")).toBe(0);
	});

	test("leaves known abbreviations alone", () => {
		expect(ofRule("It was fine, etc. and then more.", "sentence-capital")).toBe(0);
	});

	test("leaves initialisms alone", () => {
		expect(ofRule("He joined the U.S. army.", "sentence-capital")).toBe(0);
	});

	test("leaves decimals and ellipses alone", () => {
		expect(ofRule("Pi is 3.14 exactly. and more.", "sentence-capital")).toBe(0);
		expect(ofRule("Well... and then more.", "sentence-capital")).toBe(0);
	});

	test("does not treat a continuation word as an opener", () => {
		expect(ofRule("One thing. and another.", "sentence-capital")).toBe(0);
	});

	test("reports inside a blockquote", () => {
		expect(report("> quoted text here.")).toEqual(["sentence-capital@2:3"]);
	});

	test("ignores fenced code", () => {
		expect(ofRule("```\nlowercase code.\n```\n", "sentence-capital")).toBe(0);
	});

	test("ignores a heading", () => {
		expect(ofRule("# lowercase heading", "sentence-capital")).toBe(0);
	});
});

describe("punctuation-spacing", () => {
	test("reports a missing space after a stop", () => {
		expect(report("One,two is fine.")).toEqual(["punctuation-spacing@4:4"]);
	});

	test("reports a space before punctuation", () => {
		expect(report("One , two is fine.")).toEqual(["punctuation-spacing@3:5"]);
	});

	test("leaves decimals alone", () => {
		expect(ofRule("Pi is 3.14 exactly.", "punctuation-spacing")).toBe(0);
	});

	test("leaves digit groups alone", () => {
		expect(ofRule("It cost 1,000 dollars.", "punctuation-spacing")).toBe(0);
	});

	test("leaves clock times alone", () => {
		expect(ofRule("Meet at 10:30 sharp.", "punctuation-spacing")).toBe(0);
	});

	test("leaves closing brackets hugging a stop alone", () => {
		expect(ofRule("See the note (see fig. 3) for more.", "punctuation-spacing")).toBe(0);
	});

	test("leaves a trailing stop alone", () => {
		expect(ofRule("That is all.", "punctuation-spacing")).toBe(0);
	});

	test("does not touch a URL", () => {
		expect(ofRule("See https://example.com/a.b for more.", "punctuation-spacing")).toBe(0);
	});

	test("does not touch a link destination", () => {
		expect(ofRule("See [the docs](https://example.com/a.b) here.", "punctuation-spacing")).toBe(0);
	});

	test("does not touch inline code", () => {
		expect(ofRule("Use `a,b` here.", "punctuation-spacing")).toBe(0);
	});
});

describe("paragraph-single-spaced", () => {
	test("reports a wrapped paragraph line", () => {
		expect(ofRule("One two\nthree four", "paragraph-single-spaced")).toBe(1);
	});

	test("leaves a hard break alone", () => {
		expect(ofRule("one two  \nthree four", "paragraph-single-spaced")).toBe(0);
	});

	test("leaves a backslash hard break alone", () => {
		expect(ofRule("one two\\\nthree four", "paragraph-single-spaced")).toBe(0);
	});

	test("leaves a setext underline alone", () => {
		expect(ofRule("A heading\n===\nfollowing text", "paragraph-single-spaced")).toBe(0);
	});

	test("leaves a line that opens a span alone", () => {
		expect(ofRule('He said "\ncome in"', "paragraph-single-spaced")).toBe(0);
	});

	test("leaves a blockquote marker alone", () => {
		expect(ofRule("> one two\n> three four", "paragraph-single-spaced")).toBe(0);
	});

	test("ignores a list item", () => {
		expect(ofRule("- one two\n- three four", "paragraph-single-spaced")).toBe(0);
	});

	test("ignores fenced code", () => {
		expect(ofRule("```\none two\nthree four\n```", "paragraph-single-spaced")).toBe(0);
	});
});

describe("paragraph-over-spaced", () => {
	test("reports two blank lines between paragraphs", () => {
		expect(ofRule("One\n\n\ntwo", "paragraph-over-spaced")).toBe(1);
	});

	test("ignores a single blank line", () => {
		expect(ofRule("one\n\ntwo", "paragraph-over-spaced")).toBe(0);
	});

	test("ignores a blank line inside fenced code", () => {
		expect(ofRule("```\none\n\n\ntwo\n```", "paragraph-over-spaced")).toBe(0);
	});

	test("ignores a blank line inside a table", () => {
		expect(ofRule("| a | b |\n| - | - |\n| 1 | 2 |", "paragraph-over-spaced")).toBe(0);
	});

	test("ignores a blank line inside a blockquote", () => {
		expect(ofRule("> a\n>\n>\n> b", "paragraph-over-spaced")).toBe(0);
	});

	test("ignores a blank line inside a nested blockquote", () => {
		expect(ofRule("> > a\n> >\n> >\n> > b", "paragraph-over-spaced")).toBe(0);
	});

	test("still reports between list items", () => {
		expect(ofRule("- a\n\n\n- b", "paragraph-over-spaced")).toBe(1);
	});
});

describe("prose-numbers", () => {
	test("spells out a small cardinal", () => {
		expect(report("I have 3 cats.")).toEqual(["prose-numbers@7:8"]);
	});

	test("spells out a small ordinal", () => {
		expect(fixIssues("The 3rd time.", () => true)).toBe("The third time.");
	});

	test("leaves larger numbers alone", () => {
		expect(ofRule("I have 30 cats.", "prose-numbers")).toBe(0);
	});

	test("leaves numbers with units alone", () => {
		expect(ofRule("It weighs 5 kg in total.", "prose-numbers")).toBe(0);
		expect(ofRule("It costs $5 exactly.", "prose-numbers")).toBe(0);
		expect(ofRule("It rose by 5%.", "prose-numbers")).toBe(0);
	});

	test("leaves decimals alone", () => {
		expect(ofRule("Pi is 3.14 exactly.", "prose-numbers")).toBe(0);
	});

	test("leaves dates and ranges alone", () => {
		expect(ofRule("Between 1990-1995 it snowed.", "prose-numbers")).toBe(0);
	});

	test("leaves labels alone", () => {
		expect(ofRule("See chapter 3 for more.", "prose-numbers")).toBe(0);
		expect(ofRule("Turn to page 12 now.", "prose-numbers")).toBe(0);
	});

	test("leaves a version alone", () => {
		expect(ofRule("Install version 2 of the app.", "prose-numbers")).toBe(0);
	});

	test("ignores fenced code", () => {
		expect(ofRule("```\n3 cats\n```", "prose-numbers")).toBe(0);
	});

	test("ignores a heading", () => {
		expect(ofRule("# 3 ways to do it", "prose-numbers")).toBe(0);
	});
});

describe("scan", () => {
	test("classifies lines into blocks", () => {
		expect(report("# Head\n\nSome para.\n\n- item\n\n> A quote\n\n```\ncode\n```\n")).toEqual([]);
	});

	test("masks a whole table", () => {
		expect(report("| a | b |\n| - | - |\n| one,two | x |")).toEqual([]);
	});

	test("issues are ordered by position", () => {
		const issues = analyze("this is 3 cats, and that is wrong.");
		const starts = issues.map((issue) => issue.start);
		expect(starts).toEqual([...starts].toSorted((a, b) => a - b));
	});
});

describe("fixIssues", () => {
	test("applies only the selected rule", () => {
		const fixed = fixIssues("this is 3 cats, and that is wrong.", (issue) => issue.rule === "prose-numbers");
		expect(fixed).toBe("this is three cats, and that is wrong.");
	});

	test("is idempotent", () => {
		const once = fixIssues("this is 3 cats, and that is wrong.", () => true);
		expect(fixIssues(once, () => true)).toBe(once);
	});

	test("drops overlapping edits rather than interleaving them", () => {
		const fixed = fixIssues("a,b,c", (issue) => issue.rule === "punctuation-spacing");
		expect(fixed).toBe("a, b, c");
	});
});

describe("regressions", () => {
	test("does not rewrite a fenced code block", () => {
		const text = "```py\nx = 1,2\nlowercase.\n```\n\nthis is 3 cats.";
		expect(fixIssues(text, () => true)).toBe("```py\nx = 1,2\nlowercase.\n```\n\nThis is three cats.");
	});

	test("does not rewrite a link destination", () => {
		const text = "See [the docs](https://example.com/a.b) and 2 more.";
		expect(fixIssues(text, () => true)).toBe("See [the docs](https://example.com/a.b) and two more.");
	});
});
