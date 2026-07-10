import path from "node:path";
import { $ } from "bun";
import ejs from "ejs";

const CONTEXT_LINES = 3;

const res: OxlintReport = await $`bunx oxlint . --format=json`.quiet().nothrow().json();

const escapeHtml = (str: string) =>
	str
		.replaceAll("&", "&amp;")
		.replaceAll("<", "&lt;")
		.replaceAll(">", "&gt;")
		.replaceAll('"', "&quot;")
		.replaceAll("'", "&#39;");

const getSnippet = async (filename: string, span: Span) => {
	const file = await Bun.file(filename.replaceAll("%20", " ").replace("file:///", "")).text();
	const lines = file.split(/\r?\n/);

	console.log(`Read ${lines.length} lines from ${filename}`);

	const target = span.line;
	const start = Math.max(1, target - CONTEXT_LINES);
	const end = Math.min(lines.length, target + CONTEXT_LINES);

	const snipLines = [];
	for (let ln = start; ln <= end; ln++) {
		const text = lines[ln - 1] ?? "";
		const isTarget = ln === target;

		let before = escapeHtml(text);
		let highlighted = "";
		let after = "";

		if (isTarget) {
			const hlStart = span.column - 1;
			const hlEnd = hlStart + span.length;
			before = escapeHtml(text.slice(0, hlStart));
			highlighted = escapeHtml(text.slice(hlStart, hlEnd));
			after = escapeHtml(text.slice(hlEnd));
		}

		snipLines.push({
			number: ln,
			isTarget,
			before,
			highlighted,
			after,
		});
	}

	return { lines: snipLines, missing: false };
};

const buildVM = async (report: OxlintReport) => {
	console.log(`Linting ${report.number_of_files} files`);

	const promises = report.diagnostics.map(async (d) => {
		const primary = d.labels[0]?.span;
		const snippet = primary ? await getSnippet(d.filename, primary) : { lines: [], missing: true };
		return {
			...d,
			snippet,
			ruleId: d.code.replaceAll(/^eslint\(|\)$/g, ""),
			lineInfo: primary ? `${d.filename}:${primary.line}:${primary.column}` : d.filename,
		};
	});

	const diagnostics = await Promise.all(promises);

	const summary = {
		errors: diagnostics.filter((d) => d.severity === "error").length,
		warnings: diagnostics.filter((d) => d.severity === "warning").length,
		info: diagnostics.filter((d) => d.severity !== "error" && d.severity !== "warning").length,
		total: diagnostics.length,
		linted: report.number_of_files,
		rulesRan: report.number_of_rules,
		duration: (report.start_time * 1000).toFixed(2),
	};

	return { diagnostics, summary };
};

const server = Bun.serve({
	port: 3000,
	async fetch(req) {
		const url = new URL(req.url);
		if (url.pathname !== "/") {
			return new Response("Not found", { status: 404 });
		}

		try {
			const vm = await buildVM(res);
			const tplPath = path.join(import.meta.dir, "templates", "report.ejs");
			const html = await ejs.renderFile(tplPath, vm, { async: true });
			return new Response(html, { headers: { "Content-Type": "text/html" } });
		} catch (err) {
			const e = Error.isError(err) ? err.message : typeof err === "string" ? err : "Unknown error";
			return new Response(`Internal Server Error: ${e}`, { status: 500 });
		}
	},
});

console.log(`🔍 Oxlint report server running at http://localhost:${server.port}`);

interface Span {
	offset: number;
	length: number;
	line: number;
	column: number;
}

interface Label {
	span: Span;
	label?: string;
}

interface Diagnostic {
	message: string;
	code: string;
	severity: "error" | "warning" | (string & { __: never });
	causes: string[];
	url: string;
	help: string;
	filename: string;
	labels: Label[];
	related: unknown[];
}

interface OxlintReport {
	diagnostics: Diagnostic[];
	number_of_files: number;
	number_of_rules: number;
	threads_count: number;
	start_time: number;
}
