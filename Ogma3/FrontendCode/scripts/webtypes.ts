import path, { basename, dirname, join } from "node:path";
import { Glob } from "bun";
import ct from "chalk-template";
import ts from "typescript";
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

function extractComponentsFromFile(filePath: string, sourceCode: string): ComponentInfo[] {
	const components: ComponentInfo[] = [];
	const sourceFile = ts.createSourceFile(filePath, sourceCode, ts.ScriptTarget.Latest, true);

	const base = basename(filePath);

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
				// Direct string literal
				tagName = tagNameArg.text;
			} else if (ts.isIdentifier(tagNameArg)) {
				// Handle variables - try to find the variable declaration and its value
				const varName = tagNameArg.escapedText as string;
				let found = false;

				// Look for variable declarations in the file
				ts.forEachChild(sourceFile, (node) => {
					if (!found && ts.isVariableStatement(node)) {
						for (const decl of node.declarationList.declarations) {
							if (ts.isIdentifier(decl.name) && decl.name.escapedText === varName) {
								// Handle initializer with possible type assertion
								let initializer = decl.initializer;

								// Check if it's a type assertion like "x as const"
								if (initializer && ts.isAsExpression(initializer)) {
									initializer = initializer.expression;
								}

								// Extract the string value
								if (initializer && ts.isStringLiteral(initializer)) {
									tagName = initializer.text;
									found = true;
								}
							}
						}
					}
				});

				if (!found) {
					console.warn(`{dim [${base}]} Couldn't resolve variable value for tag name: {red ${varName}}`);
				}
			} else {
				console.warn(
					`{dim [${base}]} Skipping customElement call: {red Tag name is not a string literal or resolvable variable.}`,
				);
				return;
			}

			// Change to store both name and type
			const attributes: Array<{ name: string; type: string }> = [];

			if (ts.isObjectLiteralExpression(propsArg)) {
				for (const prop of propsArg.properties) {
					if (ts.isPropertyAssignment(prop) && ts.isIdentifier(prop.name)) {
						const propName = prop.name.escapedText as string;
						let typeName = "unknown";

						// Determine the type based on the initializer node kind
						if (ts.isStringLiteral(prop.initializer)) {
							typeName = "string";
						} else if (ts.isNumericLiteral(prop.initializer)) {
							typeName = "number";
						} else if (
							prop.initializer.kind === ts.SyntaxKind.TrueKeyword ||
							prop.initializer.kind === ts.SyntaxKind.FalseKeyword
						) {
							typeName = "boolean";
						} else if (ts.isArrayLiteralExpression(prop.initializer)) {
							typeName = "array";
						} else if (ts.isObjectLiteralExpression(prop.initializer)) {
							typeName = "object";
						} else if (ts.isIdentifier(prop.initializer)) {
							// Handle references to constructors or other identifiers
							const name = prop.initializer.escapedText as string;
							switch (name) {
								case "String":
									typeName = "string";
									break;
								case "Number":
									typeName = "number";
									break;
								case "Boolean":
									typeName = "boolean";
									break;
								case "Array":
									typeName = "array";
									break;
								case "Object":
									typeName = "object";
									break;
								default:
									typeName = name.toLowerCase();
							}
						}

						attributes.push({ name: propName, type: typeName });
					} else if (ts.isShorthandPropertyAssignment(prop)) {
						const propName = prop.name.escapedText as string;
						let typeName = "unknown";

						// For shorthand properties, we need a different approach
						// Try to find the variable declaration to determine its type
						let found = false;
						ts.forEachChild(sourceFile, (node) => {
							if (!found && ts.isVariableStatement(node)) {
								for (const decl of node.declarationList.declarations) {
									if (
										ts.isIdentifier(decl.name) &&
										decl.name.escapedText === propName &&
										decl.initializer
									) {
										found = true;

										// Determine type from initializer
										if (ts.isStringLiteral(decl.initializer)) {
											typeName = "string";
										} else if (ts.isNumericLiteral(decl.initializer)) {
											typeName = "number";
										} else if (
											decl.initializer.kind === ts.SyntaxKind.TrueKeyword ||
											decl.initializer.kind === ts.SyntaxKind.FalseKeyword
										) {
											typeName = "boolean";
										} else if (ts.isArrayLiteralExpression(decl.initializer)) {
											typeName = "array";
										} else if (ts.isObjectLiteralExpression(decl.initializer)) {
											typeName = "object";
										}
									}
								}
							}
						});

						attributes.push({ name: propName, type: typeName });
					} else {
						console.warn(
							ct`{dim [${base}]} Non-standard property found in props definition for {bold <${tagName}>}: {red ${ts.SyntaxKind[prop.kind]}}`,
						);
					}
				}
			} else {
				console.warn(
					ct`{dim [${base}]} Skipping props for {bold <${tagName}>}: {red Second argument to customElement is not an object literal.}`,
				);
			}

			if (tagName) {
				// Format for logging
				const attributesFormatted = attributes
					.map((attr) => ct`{blue ${attr.name}:} {green ${attr.type}}`)
					.join(", ");

				console.log(
					ct`{dim [${base}]} Found component: {bold <${tagName}>} with attributes: \{ ${attributesFormatted} \}`,
				);

				components.push({
					tagName,
					attributes, // Now this contains both name and type
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
}

await generateWebTypes();
