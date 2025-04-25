export interface BiomeDiff {
	diff: Diff;
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
	delete?: Delete;
	insert?: Delete;
}

export interface Delete {
	range: number[];
}

export interface EqualLines {
	line_count: number;
}
