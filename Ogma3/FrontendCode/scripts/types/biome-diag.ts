export interface BiomeDiag {
	summary: Summary;
	diagnostics: Diagnostic[];
	command: string;
}

export interface Diagnostic {
	category: Category;
	severity: Severity;
	description: string;
	message: Message[];
	advices: Advices;
	verboseAdvices: Advices;
	location: Location;
	tags: string[];
	source: null;
}

export interface Advices {
	advices: Advice[];
}

export interface Advice {
	diff?: Diff;
	log?: (Message[] | string)[];
	frame?: Location;
}

export interface Diff {
	dictionary: string;
	ops: Op[];
}

export interface Op {
	diffOp?: DiffOp;
	equalLines?: EqualLines;
}

export interface DiffOp {
	equal?: Delete;
	insert?: Delete;
	delete?: Delete;
}

export interface Delete {
	range: number[];
}

export interface EqualLines {
	line_count: number;
}

export interface Message {
	elements: string[];
	content: string;
}

export type Category =
	| "format"
	| "lint/style/useNodejsImportProtocol"
	| "lint/correctness/noInvalidGridAreas"
	| "organizeImports"
	| "lint/suspicious/noDuplicateFontNames"
	| "lint/suspicious/noShorthandPropertyOverrides";

export interface Location {
	path: Path;
	span: number[] | null;
	sourceCode: string;
}

export interface Path {
	file: string;
}

export type Severity = "error";

export interface Summary {
	changed: number;
	unchanged: number;
	matches: number;
	duration: Duration;
	errors: number;
	warnings: number;
	skipped: number;
	suggestedFixesSkipped: number;
	diagnosticsNotPrinted: number;
}

export interface Duration {
	secs: number;
	nanos: number;
}
