// @ts-check
import defineConfig from "stylelint-define-config";
// import "@stylelint-types/stylelint-scss";

export default defineConfig({
	extends: ["stylelint-config-standard-scss"],
	rules: {
		"scss/dollar-variable-empty-line-before": null,
		"at-rule-empty-line-before": null,
		"declaration-empty-line-before": null,
	},
});
