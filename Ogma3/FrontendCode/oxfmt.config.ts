import { defineConfig } from "oxfmt";

export default defineConfig({
	useTabs: true,
	tabWidth: 4,
	printWidth: 120,
	endOfLine: "crlf",
	semi: true,
	sortImports: {
		newlinesBetween: false,
	},
	insertFinalNewline: true,
	ignorePatterns: ["**/typescript/generated", "**/.unlighthouse"],
});
