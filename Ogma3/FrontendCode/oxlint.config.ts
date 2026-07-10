import { defineConfig } from "oxlint";
export default defineConfig({
	plugins: ["typescript", "unicorn", "oxc", "promise", "import", "react", "jsx-a11y"],
	categories: {
		correctness: "error",
		suspicious: "warn",
	},
	ignorePatterns: ["**/typescript/generated", "**/.unlighthouse"],
	rules: {
		"no-param-reassign": "error",
		"default-param-last": "error",
		"no-else-return": "error",
		"require-await": "error",
		"prefer-regex-literals": "warn",
		"typescript/prefer-as-const": "error",
		"typescript/no-inferrable-types": "off",
		"typescript/prefer-enum-initializers": "off",
		"react/self-closing-comp": "error",
		"react/react-in-jsx-scope": "off",
		"react/jsx-key": "off",
		"unicorn/prefer-number-properties": "error",
		"unicorn/no-document-cookie": "off", // Let's wait until at least 95% support: https://caniuse.com/cookie-store-api
		"no-cond-assign": "off",
	},
	settings: {
		"jsx-a11y": {
			attributes: {
				for: ["for"],
				class: ["class"],
			},
		},
	},
	options: {
		typeAware: true,
	},
	env: {
		builtin: true,
	},
	overrides: [
		{
			files: ["./scripts/**/*.ts"],
			rules: {
				"eslint/no-underscore-dangle": "off",
			},
		},
	],
});
