import type { Issue } from "./types";

/** Outcome of applying a set of edits. */
export type AppliedFix = {
	text: string;
	/** Issues whose edit was actually written, after overlap resolution. */
	applied: Issue[];
};

/**
 * Apply `issues` to `text` and return the rewritten document.
 *
 * Issues are applied right to left, so every edit still addresses the offsets it
 * was reported against regardless of how much earlier edits changed the document
 * length. A running ceiling rejects any edit reaching into a range a later edit
 * already claimed, so overlapping issues are dropped instead of interleaved.
 * Issues whose replacement equals the original text are reported as applied but
 * leave the document untouched.
 */
export const applyIssues = (text: string, issues: readonly Issue[]): AppliedFix => {
	if (issues.length === 0) {
		return { text, applied: [] };
	}

	const ordered = issues.toSorted((a, b) => b.start - a.start || b.end - a.end);

	const seen = new Set<Issue>();
	let result = text;
	let ceiling = text.length;

	for (const issue of ordered) {
		if (issue.start < 0 || issue.end > text.length || issue.start > issue.end) {continue;}
		if (issue.end > ceiling) {
			continue;
		}

		seen.add(issue);
		result = result.slice(0, issue.start) + issue.replacement + result.slice(issue.end);
		ceiling = issue.start;
	}

	// Issues dropped by overlap resolution must not be reported as fixed, and the
	// caller's ascending order is preserved for display.
	const applied = issues.filter((issue) => seen.has(issue));

	return { text: result, applied };
};
