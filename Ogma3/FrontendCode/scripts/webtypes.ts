import { Glob } from "bun";
import ts from "typescript";
import path from "node:path";
import fs from "node:fs/promises";

// --- Configuration ---
const SRC_DIR = path.resolve(process.cwd(), "typescript/src-solid");
const OUTPUT_FILE = path.resolve(process.cwd(), "../../web-types.json");
const FILE_PATTERN = "**/*.tsx";
const PACKAGE_JSON_PATH = path.resolve(process.cwd(), "package.json");
const WEB_TYPES_SCHEMA = "https://raw.githubusercontent.com/JetBrains/web-types/master/schema/web-types.json";
// --- End Configuration ---

interface ComponentInfo {
	tagName: string;
	attributes: string[];
	sourceFile: string;
}

interface WebTypeAttribute {
	name: string;
	description?: string;
	value?: {
		kind: string;
		type: string;
	};
	default?: string;
	required?: boolean;
}

interface WebTypeElement {
	name: string;
	description?: string;
	"doc-url"?: string;
	attributes: WebTypeAttribute[];
	source?: {
		module: string;
		symbol: string;
	};
}

interface WebTypesJson {
	$schema: string;
	framework: string;
	name: string;
	version: string;
	"js-types-syntax"?: string;
	"description-markup"?: string;
	contributions: {
		html: {
			elements?: WebTypeElement[];
			attributes?: WebTypeAttribute[];
		};
	};
}

async function getPackageInfo(): Promise<{ name: string; version: string }> {
	try {
		const content = await fs.readFile(PACKAGE_JSON_PATH, "utf-8");
		const pkg = JSON.parse(content);
		return {
			name: pkg.name || "unknown-package",
			version: pkg.version || "0.0.0",
		};
	} catch (error) {
		console.warn(`Could not read ${PACKAGE_JSON_PATH}. Using default name/version.`);
		console.warn(`Error: ${error instanceof Error ? error.message : error}`);
		return {
			name: "my-solid-components",
			version: "1.0.0",
		};
	}
}

function extractComponentsFromFile(filePath: string, sourceCode: string): ComponentInfo[] {
	const components: ComponentInfo[] = [];
	const sourceFile = ts.createSourceFile(filePath, sourceCode, ts.ScriptTarget.Latest, true);

	function visit(node: ts.Node) {
		if (
			ts.isCallExpression(node) &&
			ts.isIdentifier(node.expression) &&
			node.expression.escapedText === "customElement" &&
			node.arguments.length >= 2
		) {
			const tagNameArg = node.arguments[0];
			const propsArg = node.arguments[1];

			let tagName = "";
			if (ts.isStringLiteral(tagNameArg)) {
				tagName = tagNameArg.text;
			} else {
				console.warn(`[${filePath}] Skipping customElement call: Tag name is not a string literal.`);
				return;
			}

			const attributes: string[] = [];
			if (ts.isObjectLiteralExpression(propsArg)) {
				for (const prop of propsArg.properties) {
					if (ts.isPropertyAssignment(prop) && ts.isIdentifier(prop.name)) {
						attributes.push(prop.name.escapedText as string);
					} else if (ts.isShorthandPropertyAssignment(prop)) {
						attributes.push(prop.name.escapedText as string);
					} else {
						console.warn(
							`[${filePath}] Non-standard property found in props definition for <${tagName}>: ${ts.SyntaxKind[prop.kind]}`,
						);
					}
				}
			} else {
				console.warn(
					`[${filePath}] Skipping props for <${tagName}>: Second argument to customElement is not an object literal.`,
				);
			}

			if (tagName) {
				console.log(`[${filePath}] Found component: <${tagName}> with attributes: ${attributes.join(", ")}`);
				components.push({
					tagName,
					attributes,
					sourceFile: path.relative(process.cwd(), filePath),
				});
			}
		}

		ts.forEachChild(node, visit);
	}

	visit(sourceFile);
	return components;
}

async function generateWebTypes() {
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
		return;
	}

	const packageInfo = await getPackageInfo();

	const webTypesElements: WebTypeElement[] = allComponents.map((comp) => ({
		name: comp.tagName,
		description: `Custom element <${comp.tagName}> defined in ${comp.sourceFile}`,
		attributes: comp.attributes.map((attrName) => ({
			name: attrName,
			value: {
				kind: "expression",
				type: "string",
			},
		})),
		source: {
			module: `./${comp.sourceFile}`,
			symbol: comp.tagName,
		},
	}));

	const webTypesJson: WebTypesJson = {
		$schema: WEB_TYPES_SCHEMA,
		framework: "solid",
		name: packageInfo.name,
		version: packageInfo.version,
		"js-types-syntax": "typescript",
		"description-markup": "markdown",
		contributions: {
			html: {
				elements: webTypesElements.sort((a, b) => a.name.localeCompare(b.name)),
			},
		},
	};

	try {
		await Bun.write(OUTPUT_FILE, JSON.stringify(webTypesJson, null, 2));
		console.log(`\nSuccessfully generated ${OUTPUT_FILE}`);
	} catch (error) {
		console.error(`Error writing ${OUTPUT_FILE}:`, error);
	}
}

await generateWebTypes();
