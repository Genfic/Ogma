import { applyIssues } from "./apply";
import { paragraphOverSpaced } from "./rules/paragraph-over-spaced";
import { paragraphSingleSpaced } from "./rules/paragraph-single-spaced";
import { proseNumbers } from "./rules/prose-numbers";
import { punctuationSpacing } from "./rules/punctuation-spacing";
import { sentenceCapital } from "./rules/sentence-capital";
import { scanMarkdown } from "./scan";
import type { Issue, IssueRule, MarkdownScan, RuleId } from "./types";

export { applyIssues } from "./apply";
export { scanMarkdown } from "./scan";
export * from "./types";

/** Every rule, in the order issues are reported. */
export const RULES: readonly IssueRule[] = [
	sentenceCapital,
	punctuationSpacing,
	paragraphSingleSpaced,
	paragraphOverSpaced,
	proseNumbers,
];

/** A rule lookup by id. */
export const RULE_BY_ID: ReadonlyMap<RuleId, IssueRule> = new Map(RULES.map((rule) => [rule.id, rule]));

/** Every issue in `scan`, ordered by position and then by rule. */
export const issuesOf = (scan: MarkdownScan): Issue[] =>
	RULES.flatMap((rule) => rule.detect(scan)).toSorted((a, b) => a.start - b.start || a.rule.localeCompare(b.rule));

/** Run every rule over `text`. */
export const analyze = (text: string): Issue[] => issuesOf(scanMarkdown(text));

/** Apply the issues selected by `predicate` to `text`, in one pass. */
export const fixIssues = (text: string, predicate: (issue: Issue) => boolean): string =>
	applyIssues(text, analyze(text).filter(predicate)).text;
