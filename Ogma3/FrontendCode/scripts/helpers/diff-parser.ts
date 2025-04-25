import type { BiomeDiff } from "../types/biome-diag-diff";

/**
 * Parse a diff JSON and generate HTML to visualize the changes using CSS Grid
 */
export function generateDiffHtml(diffJson: BiomeDiff) {
	const diffData = diffJson.diff;
	const dictionary = diffData.dictionary;
	const ops = diffData.ops;

	// Initialize original and modified text
	let originalText = "";
	let modifiedText = "";

	// Process each operation to build original and modified text
	for (const op of ops) {
		if (op.diffOp) {
			const diffOp = op.diffOp;

			if (diffOp.equal) {
				const range = diffOp.equal.range;
				const text = dictionary.substring(range[0], range[1]);
				originalText += text;
				modifiedText += text;
			} else if (diffOp.delete) {
				const range = diffOp.delete.range;
				const text = dictionary.substring(range[0], range[1]);
				originalText += text;
			} else if (diffOp.insert) {
				const range = diffOp.insert.range;
				const text = dictionary.substring(range[0], range[1]);
				modifiedText += text;
			}
		} else if (op.equalLines) {
			// For equal lines, add placeholders
			const lineCount = op.equalLines.line_count;
			const placeholder = `[${lineCount} unchanged lines]`;
			originalText += `\n${placeholder}\n`;
			modifiedText += `\n${placeholder}\n`;
		}
	}

	const mapLine = (line: string) => {
		const found = /^\[(\d+) unchanged lines]$/.exec(line);
		const num = found ? Number.parseInt(found[1]) : null;
		return found && num ? { text: line, cls: "skip", lines: num } : line;
	};

	// Generate line-by-line diff for better visualization
	const originalLines = originalText.split("\n").map(mapLine);
	const modifiedLines = modifiedText.split("\n").map(mapLine);

	// Create HTML for the diff visualization using CSS Grid
	let diffHtml = `
    <div class="diff-container">
      <div class="diff-header">
        <div class="diff-header-cell line-number">Line</div>
        <div class="diff-header-cell original-content">Original</div>
        <div class="diff-header-cell line-number">Line</div>
        <div class="diff-header-cell modified-content">Modified</div>
      </div>
  `;

	// Find the maximum number of lines
	const maxLines = Math.max(originalLines.length, modifiedLines.length);

	let lineNumber = 1;

	// Process each line
	for (let i = 0; i < maxLines; i++) {
		let originalLine = "";
		let modifiedLine = "";

		let cls = "";
		let lines = 1;

		if (i < originalLines.length) {
			const l = originalLines[i];
			if (typeof l === "string") {
				originalLine = l;
			} else if (l.text) {
				originalLine = l.text;
				cls = l.cls;
				lines = l.lines;
			}
		}

		if (i < modifiedLines.length) {
			const l = modifiedLines[i];
			if (typeof l === "string") {
				modifiedLine = l;
			} else if (l.text) {
				modifiedLine = l.text;
				cls = l.cls;
				lines = l.lines;
			}
		}

		// Determine line status - added, removed, or unchanged
		let lineStatus = "unchanged";
		if (originalLine !== modifiedLine) {
			if (!originalLine) {
				lineStatus = "added";
			} else if (!modifiedLine) {
				lineStatus = "removed";
			} else {
				lineStatus = "changed";
			}
		}

		const line = lines > 1 ? `${lineNumber}-${lineNumber + lines - 1}` : lineNumber;

		diffHtml += `
      <div class="diff-row ${lineStatus} ${cls}">
        <div class="diff-cell line-number">${line}</div>
        <div class="diff-cell original-content">${escapeHtml(originalLine)}</div>
        <div class="diff-cell line-number">${line}</div>
        <div class="diff-cell modified-content">${escapeHtml(modifiedLine)}</div>
      </div>
    `;

		// Add line numbers
		lineNumber += lines;
	}

	diffHtml += "</div>";

	return diffHtml;
}

/**
 * Helper function to escape HTML special characters
 */
function escapeHtml(text: string) {
	if (!text) return "";
	return text
		.replace(/&/g, "&amp;")
		.replace(/</g, "&lt;")
		.replace(/>/g, "&gt;")
		.replace(/"/g, "&quot;")
		.replace(/'/g, "&#039;");
}

// For use in an EJS template:
// <%- generateDiffHtml(diffJson) %>
