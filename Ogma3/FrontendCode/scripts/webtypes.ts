import path, { basename, dirname, join } from "node:path";
import { Glob } from "bun";
import ct from "chalk-template";
import { kebabCase } from "es-toolkit";
import type { Expression } from "oxc-parser";
import { parseSync, Visitor } from "oxc-parser";
import pkg from "../package.json" with { type: "json" };
import { alphaBy } from "./helpers/function-helpers";
import type { HTMLAttribute, HTMLElement, Webtypes } from "./types/webtypes";

const _root = dirname(Bun.main);

// --- Configuration ---
const SRC_DIR = join(_root, "..", "typescript", "src");
const OUTPUT_FILE = join(_root, "..", "web-types.json");
const FILE_PATTERN = "**/*.tsx";
const WEB_TYPES_SCHEMA = "https://raw.githubusercontent.com/JetBrains/web-types/master/schema/web-types.json";
// --- End Configuration ---

interface ComponentInfo {
	tagName: string;
	attributes: { name: string; type: string }[];
	sourceFile: string;
}

function getPackageInfo(): { name: string; version: string } {
	try {
		return {
			name: pkg.name || "unknown-package",
			version: pkg.version || "0.0.0",
		};
	} catch (error) {
		console.warn(ct`{red Could not read package.json. Using default name/version.}`);
		console.warn(ct`{red Error: {bold ${error instanceof Error ? error.message : error}}}`);
		return {
			name: "my-solid-components",
			version: "1.0.0",
		};
	}
}

/** Unwraps `x as Foo`, `x as const`, and `x satisfies Foo` down to the underlying expression. */
function unwrapTypeWrapper(node: Expression): Expression {
	if (node.type === "TSAsExpression" || node.type === "TSSatisfiesExpression") {
		return unwrapTypeWrapper(node.expression);
	}
	return node;
}

/** Resolves the type of a property's value expression directly, e.g. `foo: "bar"` or `foo: String`. */
function resolveDirectType(node: Expression): string {
	switch (node.type) {
		case "Literal": {
			if (typeof node.value === "string") {
				return "string";
			}
			if (typeof node.value === "number") {
				return "number";
			}
			if (typeof node.value === "boolean") {
				return "boolean";
			}
			return "unknown";
		}
		case "ArrayExpression": {
			return "array";
		}
		case "ObjectExpression": {
			return "object";
		}
		case "Identifier": {
			switch (node.name) {
				case "String":
					return "string";
				case "Number":
					return "number";
				case "Boolean":
					return "boolean";
				case "Array":
					return "array";
				case "Object":
					return "object";
				default:
					return node.name.toLowerCase();
			}
		}
		default: {
			return "unknown";
		}
	}
}

/** Resolves the type of a shorthand property, e.g. `{ foo }`, via its top-level variable declaration. */
function resolveShorthandType(propName: string, topLevelVars: Map<string, Expression>): string {
	const initializer = topLevelVars.get(propName);
	if (!initializer) {
		return "unknown";
	}

	const node = unwrapTypeWrapper(initializer);

	switch (node.type) {
		case "Literal": {
			if (typeof node.value === "string") {
				return "string";
			}
			if (typeof node.value === "number") {
				return "number";
			}
			if (typeof node.value === "boolean") {
				return "boolean";
			}
			return "unknown";
		}
		case "ArrayExpression": {
			return "array";
		}
		case "ObjectExpression": {
			return "object";
		}
		default: {
			return "unknown";
		}
	}
}

/** Resolves the tag name argument: either a direct string literal or an identifier pointing at one. */
function resolveTagName(arg: Expression, topLevelVars: Map<string, Expression>, base: string): string {
	if (arg.type === "Literal" && typeof arg.value === "string") {
		return arg.value;
	}

	if (arg.type === "Identifier") {
		const initializer = topLevelVars.get(arg.name);
		const resolved = initializer ? unwrapTypeWrapper(initializer) : null;

		if (resolved && resolved.type === "Literal" && typeof resolved.value === "string") {
			return resolved.value;
		}

		console.warn(ct`{dim [${base}]} Couldn't resolve variable value for tag name: {red ${arg.name}}`);
		return "";
	}

	console.warn(
		ct`{dim [${base}]} Skipping component call: {red Tag name is not a string literal or resolvable variable.}`,
	);
	return "";
}

function extractComponentsFromFile(filePath: string, sourceCode: string): ComponentInfo[] {
	const components: ComponentInfo[] = [];
	const base = basename(filePath);

	// Language is inferred from the .tsx extension on filePath.
	const { program, errors } = parseSync(filePath, sourceCode, {
		sourceType: "module",
	});

	for (const error of errors) {
		console.warn(ct`{dim [${base}]} Parse error: {red ${error.message}}`);
	}

	// Top-level `const`/`let` declarations, so identifiers used as tag names or shorthand
	// prop values can be resolved back to their initializers. This mirrors the old script's
	// ts.forEachChild(sourceFile, ...) lookup, which was also top-level only.
	const topLevelVars = new Map<string, Expression>();
	for (const stmt of program.body) {
		if (stmt.type === "VariableDeclaration") {
			for (const decl of stmt.declarations) {
				if (decl.id.type === "Identifier" && decl.init) {
					topLevelVars.set(decl.id.name, decl.init);
				}
			}
		}
	}

	const visitor = new Visitor({
		CallExpression(node) {
			if (node.callee.type !== "Identifier" || node.callee.name !== "component" || node.arguments.length < 2) {
				return;
			}

			const [tagNameArg, propsArg] = node.arguments;

			if (tagNameArg.type === "SpreadElement" || propsArg.type === "SpreadElement") {
				console.warn(ct`{dim [${base}]} Skipping component call: {red Arguments cannot be spread elements.}`);
				return;
			}

			const tagName = resolveTagName(tagNameArg, topLevelVars, base);
			if (!tagName) {
				return;
			}

			const attributes: Array<{ name: string; type: string }> = [];

			if (propsArg.type === "ObjectExpression") {
				for (const prop of propsArg.properties) {
					if (
						prop.type === "Property" &&
						prop.kind === "init" &&
						!prop.computed &&
						!prop.method &&
						prop.key.type === "Identifier"
					) {
						const propName = prop.key.name;

						if (prop.shorthand) {
							// Preserves the original script's behavior of NOT kebab-casing
							// shorthand property names (only the non-shorthand branch did) —
							// flag if that asymmetry was unintentional.
							attributes.push({ name: propName, type: resolveShorthandType(propName, topLevelVars) });
						} else {
							attributes.push({ name: kebabCase(propName), type: resolveDirectType(prop.value) });
						}
					} else {
						console.warn(
							ct`{dim [${base}]} Non-standard property found in props definition for {bold <${tagName}>}: {red ${prop.type}}`,
						);
					}
				}
			} else {
				console.warn(
					ct`{dim [${base}]} Skipping props for {bold <${tagName}>}: {red Second argument to customElement is not an object literal.}`,
				);
			}

			const attributesFormatted = attributes
				.map((attr) => ct`{blue ${attr.name}:} {green ${attr.type}}`)
				.join(", ");

			console.log(
				ct`{dim [${base}]} Found component: {bold <${tagName}>} with attributes: \{ ${attributesFormatted} \}`,
			);

			components.push({
				tagName,
				attributes,
				sourceFile: path.relative(process.cwd(), filePath),
			});
		},
	});

	visitor.visit(program);

	return components;
}

console.log(`Scanning for components in ${SRC_DIR} using pattern: ${FILE_PATTERN}`);
const glob = new Glob(FILE_PATTERN);
const allComponents: ComponentInfo[] = [];
let fileCount = 0;

for await (const file of glob.scan(SRC_DIR)) {
	fileCount++;
	const filePath = path.join(SRC_DIR, file);
	try {
		const fileContent = await Bun.file(filePath).text();
		const componentsInFile = extractComponentsFromFile(filePath, fileContent);
		allComponents.push(...componentsInFile);
	} catch (error) {
		console.error(`Error processing file ${filePath}:`, error);
	}
}

console.log(`\nScanned ${fileCount} files. Found ${allComponents.length} components.`);

if (allComponents.length === 0) {
	console.log("No components found matching the criteria. Exiting.");
	process.exit(0);
}

const packageInfo = getPackageInfo();

const webTypesElements: HTMLElement[] = allComponents.map((comp) => ({
	name: comp.tagName,
	description: `Custom element <${comp.tagName}> defined in ${comp.sourceFile}`,
	attributes: comp.attributes.map(
		(attr): HTMLAttribute => ({
			name: attr.name,
			value: {
				kind: "expression",
				type: attr.type,
			},
		}),
	),
	source: {
		module: `./${comp.sourceFile}`,
		symbol: comp.tagName,
	},
}));

const webTypesJson: Webtypes = {
	$schema: WEB_TYPES_SCHEMA,
	name: packageInfo.name,
	version: packageInfo.version,
	"js-types-syntax": "typescript",
	"description-markup": "markdown",
	contributions: {
		html: {
			elements: webTypesElements.sort(alphaBy((e) => e.name)),
		},
	},
};

try {
	await Bun.write(OUTPUT_FILE, JSON.stringify(webTypesJson, null, 2));
	console.log(`\nSuccessfully generated ${OUTPUT_FILE}`);
} catch (error) {
	console.error(`Error writing ${OUTPUT_FILE}:`, error);
}
