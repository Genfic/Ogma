export interface Config {
	js: Entry | undefined;
	css: Entry | undefined;
}

interface Entry {
	in: string[];
	out: string;
}