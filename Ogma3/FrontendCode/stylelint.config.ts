import type { Config } from "stylelint";

/* TODO Workarounds:
 * - "stylelint-config-standard": "40.0.0" was added only to be patched, remove when 18.0 releases.
 * - "stylelint": "github:stylelint/stylelint#v18" is used to support TS config with TS7, 18.0 fixes it
 */

export default {
	extends: ["stylelint-config-standard-scss"],
	rules: {
		"scss/dollar-variable-empty-line-before": null,
		"at-rule-empty-line-before": null,
		"declaration-empty-line-before": null,
		"selector-type-no-unknown": [
			true,
			{
				ignoreTypes: ["o-icon"],
			},
		],
	},
} satisfies Config;
