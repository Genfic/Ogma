import type { Config } from "stylelint";

export default {
	extends: ["stylelint-config-standard-scss"],
	rules: {
		"scss/dollar-variable-empty-line-before": null,
		"at-rule-empty-line-before": null,
		"declaration-empty-line-before": null,
	},
} satisfies Config;
