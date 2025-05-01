export interface Webtypes {
	$schema?: string;
	/**
	 * Deprecated since 2024.2 because of ambiguous meaning - use "required-context" instead.
	 */
	context?: RequiredContext;
	"contexts-config"?: { [key: string]: ContextKindConfig };
	/**
	 * Symbol can be contributed to one of the 3 namespaces - HTML, CSS and JS. Within a
	 * particular namespace there can be different kinds of symbols. In each of the namespaces,
	 * there are several predefined kinds, which integrate directly with IDE, but providers are
	 * free to define their own.
	 */
	contributions?: Contributions;
	"default-icon"?: string;
	"description-markup"?: DescriptionMarkup;
	/**
	 * Framework, for which the components are provided by the library. If the library is not
	 * enabled in a particular context, all symbols from this file will not be available as
	 * well. If you want symbols to be always available do not specify framework.
	 */
	framework?: string;
	"framework-config"?: FrameworkConfig;
	"js-types-syntax"?: TypesSyntax;
	/**
	 * Name of the library.
	 */
	name: string;
	/**
	 * Since 2024.2. Specify contexts, which are required to enable contributions from this file.
	 */
	"required-context"?: RequiredContext;
	/**
	 * Version of the library, for which Web-Types are provided.
	 */
	version: string;
}

/**
 * Deprecated since 2024.2 because of ambiguous meaning - use "required-context" instead.
 *
 * Since 2024.2. Specify contexts, which are required to enable this contribution.
 *
 * Since 2024.2. Specify contexts, which are required to enable contributions from this file.
 */
export interface RequiredContext {
	kind?: string;
	name?: string;
	anyOf?: RequiredContext[];
	allOf?: RequiredContext[];
	not?: RequiredContext;
}

export interface ContextKindConfig {
	"disable-when"?: DisablementRules;
	"enable-when"?: EnablementRules;
	/**
	 * Context kind. Only a single context of the particular kind will be enabled. An example of
	 * context kind is framework, which has dedicated support in Web Types.
	 */
	kind?: string;
	[property: string]: ContextConfig;
}

/**
 * Specify rules for disabling web framework support. These rules take precedence over
 * enable-when rules. They allow to turn off framework support in case of some conflicts
 * between frameworks priority.
 */
export interface DisablementRules {
	/**
	 * Extensions of files, which should have the framework support disabled
	 */
	"file-extensions"?: string[];
	/**
	 * RegExp patterns to match file names, which should have the framework support disabled
	 */
	"file-name-patterns"?: Pattern[];
}

/**
 * A RegEx pattern to match whole content. Syntax should work with at least ECMA, Java and
 * Python implementations.
 */
export type Pattern = PatternObject | string;

export interface PatternObject {
	"case-sensitive"?: boolean;
	regex?: string;
	[property: string]: unknown;
}

/**
 * Specify rules for enabling web framework support. Only one framework can be enabled in a
 * particular file. If you need your contributions to be enabled in all files, regardless of
 * the context, do not specify the framework.
 */
export interface EnablementRules {
	/**
	 * Extensions of files, which should have the framework support enabled. Use this to support
	 * custom file extensions like '.vue' or '.svelte'. Never specify generic extensions like
	 * '.html', '.js' or '.ts'. If you need your contributions to be present in every file don't
	 * specify the framework at all
	 */
	"file-extensions"?: string[];
	/**
	 * RegExp patterns to match file names, which should have the framework support enabled. Use
	 * carefully as broken pattern may even freeze IDE.
	 */
	"file-name-patterns"?: Pattern[];
	/**
	 * Global JavaScript libraries names enabled within the IDE, which enable framework support
	 * in the whole project
	 */
	"ide-libraries"?: string[];
	/**
	 * Node.js package names, which enable framework support within the folder containing the
	 * package.json.
	 */
	"node-packages"?: string[];
	/**
	 * List of tool executables (without extension), which presence should be checked in the
	 * project. In case of Node projects, such tools will be searched in node_modules/.bin/
	 */
	"project-tool-executables"?: string[];
	/**
	 * Since 2024.2. Ruby gem names, which enable framework support within the particular Ruby
	 * module.
	 */
	"ruby-gems"?: string[];
	[property: string]: string[];
}

/**
 * Since 2024.2. Provide rules for setting a particular name for particular context kind.
 * This allows to contribute additional Web Types for example if a particular library is
 * present in the project.
 */
export interface ContextConfig {
	"disable-when"?: DisablementRules;
	"enable-when"?: EnablementRules;
}

/**
 * Symbol can be contributed to one of the 3 namespaces - HTML, CSS and JS. Within a
 * particular namespace there can be different kinds of symbols. In each of the namespaces,
 * there are several predefined kinds, which integrate directly with IDE, but providers are
 * free to define their own.
 */
export interface Contributions {
	css?: CSS;
	html?: HTML;
	js?: JSGlobal;
}

/**
 * The base for any contributions.
 */
export interface CSSPseudoElement {
	/**
	 * Specify whether the pseudo-element requires arguments.
	 */
	arguments?: boolean;
	abstract?: boolean;
	css?: CSS;
	deprecated?: Deprecated;
	"deprecated-since"?: string;
	description?: string;
	"description-sections"?: { [key: string]: string };
	"doc-url"?: string;
	"exclusive-contributions"?: string[];
	experimental?: Deprecated;
	extends?: Reference;
	extension?: boolean;
	html?: HTML;
	icon?: string;
	js?: JS;
	name?: string;
	obsolete?: Deprecated;
	"obsolete-since"?: string;
	pattern?: NamePatternRoot;
	priority?: Priority;
	proximity?: number;
	/**
	 * Since 2024.2. Specify contexts, which are required to enable this contribution.
	 */
	"required-context"?: RequiredContext;
	since?: string;
	source?: Source;
	virtual?: boolean;
	/**
	 * CSS classes
	 */
	classes?: CSSGenericItem[];
	/**
	 * CSS functions
	 */
	functions?: CSSGenericItem[];
	/**
	 * CSS parts
	 */
	parts?: CSSGenericItem[];
	/**
	 * CSS properties
	 */
	properties?: CSSProperty[];
	/**
	 * CSS pseudo-classes
	 */
	"pseudo-classes"?: CSSPseudoClass[];
	/**
	 * CSS pseudo-elements
	 */
	"pseudo-elements"?: CSSPseudoElement[];
	[property: string]: unknown;
}

/**
 * The base for any contributions.
 */
export interface CSSPseudoClass {
	/**
	 * Specify whether the pseudo-class requires arguments.
	 */
	arguments?: boolean;
	abstract?: boolean;
	css?: CSS;
	deprecated?: Deprecated;
	"deprecated-since"?: string;
	description?: string;
	"description-sections"?: { [key: string]: string };
	"doc-url"?: string;
	"exclusive-contributions"?: string[];
	experimental?: Deprecated;
	extends?: Reference;
	extension?: boolean;
	html?: HTML;
	icon?: string;
	js?: JS;
	name?: string;
	obsolete?: Deprecated;
	"obsolete-since"?: string;
	pattern?: NamePatternRoot;
	priority?: Priority;
	proximity?: number;
	/**
	 * Since 2024.2. Specify contexts, which are required to enable this contribution.
	 */
	"required-context"?: RequiredContext;
	since?: string;
	source?: Source;
	virtual?: boolean;
	/**
	 * CSS classes
	 */
	classes?: CSSGenericItem[];
	/**
	 * CSS functions
	 */
	functions?: CSSGenericItem[];
	/**
	 * CSS parts
	 */
	parts?: CSSGenericItem[];
	/**
	 * CSS properties
	 */
	properties?: CSSProperty[];
	/**
	 * CSS pseudo-classes
	 */
	"pseudo-classes"?: CSSPseudoClass[];
	/**
	 * CSS pseudo-elements
	 */
	"pseudo-elements"?: CSSPseudoElement[];
	[property: string]: unknown;
}

/**
 * The base for any contributions.
 */
export interface CSSProperty {
	values?: string[];
	abstract?: boolean;
	css?: CSS;
	deprecated?: Deprecated;
	"deprecated-since"?: string;
	description?: string;
	"description-sections"?: { [key: string]: string };
	"doc-url"?: string;
	"exclusive-contributions"?: string[];
	experimental?: Deprecated;
	extends?: Reference;
	extension?: boolean;
	html?: HTML;
	icon?: string;
	js?: JS;
	name?: string;
	obsolete?: Deprecated;
	"obsolete-since"?: string;
	pattern?: NamePatternRoot;
	priority?: Priority;
	proximity?: number;
	/**
	 * Since 2024.2. Specify contexts, which are required to enable this contribution.
	 */
	"required-context"?: RequiredContext;
	since?: string;
	source?: Source;
	virtual?: boolean;
	/**
	 * CSS classes
	 */
	classes?: CSSGenericItem[];
	/**
	 * CSS functions
	 */
	functions?: CSSGenericItem[];
	/**
	 * CSS parts
	 */
	parts?: CSSGenericItem[];
	/**
	 * CSS properties
	 */
	properties?: CSSProperty[];
	/**
	 * CSS pseudo-classes
	 */
	"pseudo-classes"?: CSSPseudoClass[];
	/**
	 * CSS pseudo-elements
	 */
	"pseudo-elements"?: CSSPseudoElement[];
	[property: string]: unknown;
}

/**
 * The base for any contribution, which can possibly have a JS type.
 *
 * The base for any contributions.
 */
export interface JSSymbol {
	/**
	 * Kind of the symbol. Default is variable.
	 */
	kind?: JSSymbolKind;
	type?: HTMLValueType;
	abstract?: boolean;
	css?: CSS;
	deprecated?: Deprecated;
	"deprecated-since"?: string;
	description?: string;
	"description-sections"?: { [key: string]: string };
	"doc-url"?: string;
	"exclusive-contributions"?: string[];
	experimental?: Deprecated;
	extends?: Reference;
	extension?: boolean;
	html?: HTML;
	icon?: string;
	js?: JS;
	name?: string;
	obsolete?: Deprecated;
	"obsolete-since"?: string;
	pattern?: NamePatternRoot;
	priority?: Priority;
	proximity?: number;
	/**
	 * Since 2024.2. Specify contexts, which are required to enable this contribution.
	 */
	"required-context"?: RequiredContext;
	since?: string;
	source?: Source;
	virtual?: boolean;
	/**
	 * DOM events
	 */
	events?: GenericJSContributionElement[];
	/**
	 * JavaScript properties of an object, HTML tag, framework component, etc.
	 */
	properties?: JSProperty[];
	/**
	 * Symbols available for JavaScript resolve. TypeScript resolve is not supported.
	 */
	symbols?: JSSymbol[];
	[property: string]: unknown;
}

/**
 * A generic contribution. All contributions are of this type, except for HTML attributes
 * and elements, as well as predefined CSS contribution kinds.
 *
 * The base for any contribution, which can possibly have a JS type.
 *
 * The base for any contributions.
 */
export interface JSProperty {
	/**
	 * Specifies whether the property is read only.
	 */
	"read-only"?: boolean;
	"attribute-value"?: HTMLAttributeValue;
	default?: string;
	required?: boolean;
	type?: HTMLValueType;
	abstract?: boolean;
	css?: CSS;
	deprecated?: Deprecated;
	"deprecated-since"?: string;
	description?: string;
	"description-sections"?: { [key: string]: string };
	"doc-url"?: string;
	"exclusive-contributions"?: string[];
	experimental?: Deprecated;
	extends?: Reference;
	extension?: boolean;
	html?: HTML;
	icon?: string;
	js?: JS;
	name?: string;
	obsolete?: Deprecated;
	"obsolete-since"?: string;
	pattern?: NamePatternRoot;
	priority?: Priority;
	proximity?: number;
	/**
	 * Since 2024.2. Specify contexts, which are required to enable this contribution.
	 */
	"required-context"?: RequiredContext;
	since?: string;
	source?: Source;
	virtual?: boolean;
	/**
	 * DOM events
	 */
	events?: GenericJSContributionElement[];
	/**
	 * JavaScript properties of an object, HTML tag, framework component, etc.
	 */
	properties?: JSProperty[];
	/**
	 * Symbols available for JavaScript resolve. TypeScript resolve is not supported.
	 */
	symbols?: JSSymbol[];
	[property: string]: unknown;
}

/**
 * A generic contribution. All contributions are of this type, except for HTML attributes
 * and elements, as well as predefined CSS contribution kinds.
 *
 * The base for any contribution, which can possibly have a JS type.
 *
 * The base for any contributions.
 */
export interface GenericJSContributionElement {
	"attribute-value"?: HTMLAttributeValue;
	default?: string;
	required?: boolean;
	type?: HTMLValueType;
	abstract?: boolean;
	css?: CSS;
	deprecated?: Deprecated;
	"deprecated-since"?: string;
	description?: string;
	"description-sections"?: { [key: string]: string };
	"doc-url"?: string;
	"exclusive-contributions"?: string[];
	experimental?: Deprecated;
	extends?: Reference;
	extension?: boolean;
	html?: HTML;
	icon?: string;
	js?: JS;
	name?: string;
	obsolete?: Deprecated;
	"obsolete-since"?: string;
	pattern?: NamePatternRoot;
	priority?: Priority;
	proximity?: number;
	/**
	 * Since 2024.2. Specify contexts, which are required to enable this contribution.
	 */
	"required-context"?: RequiredContext;
	since?: string;
	source?: Source;
	virtual?: boolean;
	/**
	 * DOM events
	 */
	events?: GenericJSContributionElement[];
	/**
	 * JavaScript properties of an object, HTML tag, framework component, etc.
	 */
	properties?: JSProperty[];
	/**
	 * Symbols available for JavaScript resolve. TypeScript resolve is not supported.
	 */
	symbols?: JSSymbol[];
	[property: string]: unknown;
}

/**
 * Contains contributions to JS namespace. It's property names represent symbol kinds, its
 * property values contain list of contributions of particular kind. There are 2 predefined
 * kinds, which integrate directly with IDE - properties and events.
 */
export interface JS {
	/**
	 * DOM events
	 */
	events?: GenericJSContributionElement[];
	/**
	 * JavaScript properties of an object, HTML tag, framework component, etc.
	 */
	properties?: JSProperty[];
	/**
	 * Symbols available for JavaScript resolve. TypeScript resolve is not supported.
	 */
	symbols?: JSSymbol[];
	[property: string]: unknown;
}

/**
 * A generic contribution. All contributions are of this type, except for HTML attributes
 * and elements, as well as predefined CSS contribution kinds.
 *
 * The base for any contribution, which can possibly have a JS type.
 *
 * The base for any contributions.
 */
export interface GenericHTMLContributionElement {
	"attribute-value"?: HTMLAttributeValue;
	default?: string;
	required?: boolean;
	type?: HTMLValueType;
	abstract?: boolean;
	css?: CSS;
	deprecated?: Deprecated;
	"deprecated-since"?: string;
	description?: string;
	"description-sections"?: { [key: string]: string };
	"doc-url"?: string;
	"exclusive-contributions"?: string[];
	experimental?: Deprecated;
	extends?: Reference;
	extension?: boolean;
	html?: HTML;
	icon?: string;
	js?: JS;
	name?: string;
	obsolete?: Deprecated;
	"obsolete-since"?: string;
	pattern?: NamePatternRoot;
	priority?: Priority;
	proximity?: number;
	/**
	 * Since 2024.2. Specify contexts, which are required to enable this contribution.
	 */
	"required-context"?: RequiredContext;
	since?: string;
	source?: Source;
	virtual?: boolean;
	/**
	 * HTML attributes.
	 */
	attributes?: HTMLAttribute[];
	/**
	 * HTML elements.
	 */
	elements?: HTMLElement[];
	/**
	 * DOM events are deprecated in HTML namespace. Contribute events to JS namespace: /js/events
	 */
	events?: GenericHTMLContributionElement[];
	[property: string]: unknown;
}

/**
 * The base for any contributions.
 */
export interface HTMLElement {
	abstract?: boolean;
	css?: CSS;
	deprecated?: Deprecated;
	"deprecated-since"?: string;
	description?: string;
	"description-sections"?: { [key: string]: string };
	"doc-url"?: string;
	"exclusive-contributions"?: string[];
	experimental?: Deprecated;
	extends?: Reference;
	extension?: boolean;
	html?: HTML;
	icon?: string;
	js?: JS;
	name: string;
	obsolete?: Deprecated;
	"obsolete-since"?: string;
	pattern?: NamePatternRoot;
	priority?: Priority;
	proximity?: number;
	/**
	 * Since 2024.2. Specify contexts, which are required to enable this contribution.
	 */
	"required-context"?: RequiredContext;
	since?: string;
	source?: Source;
	virtual?: boolean;
	/**
	 * HTML attributes.
	 */
	attributes?: HTMLAttribute[];
	/**
	 * HTML elements.
	 */
	elements?: HTMLElement[];
	/**
	 * DOM events are deprecated in HTML namespace. Contribute events to JS namespace: /js/events
	 */
	events?: GenericHTMLContributionElement[];
	[property: string]: unknown;
}

/**
 * The base for any contributions.
 */
export interface HTMLAttribute {
	default?: string;
	required?: boolean;
	value?: HTMLAttributeValue;
	"vue-argument"?: DeprecatedHTMLAttributeVueArgument;
	/**
	 * Deprecated vue-specific property - contribute Vue directives to
	 * /contributions/html/vue-directives
	 */
	"vue-modifiers"?: DeprecatedHTMLAttributeVueModifier[];
	abstract?: boolean;
	css?: CSS;
	deprecated?: Deprecated;
	"deprecated-since"?: string;
	description?: string;
	"description-sections"?: { [key: string]: string };
	"doc-url"?: string;
	"exclusive-contributions"?: string[];
	experimental?: Deprecated;
	extends?: Reference;
	extension?: boolean;
	html?: HTML;
	icon?: string;
	js?: JS;
	name?: string;
	obsolete?: Deprecated;
	"obsolete-since"?: string;
	pattern?: NamePatternRoot;
	priority?: Priority;
	proximity?: number;
	/**
	 * Since 2024.2. Specify contexts, which are required to enable this contribution.
	 */
	"required-context"?: RequiredContext;
	since?: string;
	source?: Source;
	virtual?: boolean;
	/**
	 * HTML attributes.
	 */
	attributes?: HTMLAttribute[];
	/**
	 * HTML elements.
	 */
	elements?: HTMLElement[];
	/**
	 * DOM events are deprecated in HTML namespace. Contribute events to JS namespace: /js/events
	 */
	events?: GenericHTMLContributionElement[];
	[property: string]: unknown;
}

/**
 * Contains contributions to HTML namespace. Its property names represent symbol kinds, its
 * property values contain list of contributions of particular kind. There are 2 predefined
 * kinds, which integrate directly with IDE - HTML elements and HTML attributes. There are
 * also 2 deprecated kinds: tags (which is equivalent to 'elements') and 'events' (which was
 * moved to JS namespace)
 */
export interface HTML {
	"description-markup"?: DescriptionMarkup;
	/**
	 * Deprecated, use `elements` property.
	 */
	tags?: HTMLElement[];
	"types-syntax"?: TypesSyntax;
	/**
	 * HTML attributes.
	 */
	attributes?: HTMLAttribute[];
	/**
	 * HTML elements.
	 */
	elements?: HTMLElement[];
	/**
	 * DOM events are deprecated in HTML namespace. Contribute events to JS namespace: /js/events
	 */
	events?: GenericHTMLContributionElement[];
	[property: string]: unknown;
}

/**
 * The base for any contributions.
 */
export interface CSSGenericItem {
	abstract?: boolean;
	css?: CSS;
	deprecated?: Deprecated;
	"deprecated-since"?: string;
	description?: string;
	"description-sections"?: { [key: string]: string };
	"doc-url"?: string;
	"exclusive-contributions"?: string[];
	experimental?: Deprecated;
	extends?: Reference;
	extension?: boolean;
	html?: HTML;
	icon?: string;
	js?: JS;
	name?: string;
	obsolete?: Deprecated;
	"obsolete-since"?: string;
	pattern?: NamePatternRoot;
	priority?: Priority;
	proximity?: number;
	/**
	 * Since 2024.2. Specify contexts, which are required to enable this contribution.
	 */
	"required-context"?: RequiredContext;
	since?: string;
	source?: Source;
	virtual?: boolean;
	/**
	 * CSS classes
	 */
	classes?: CSSGenericItem[];
	/**
	 * CSS functions
	 */
	functions?: CSSGenericItem[];
	/**
	 * CSS parts
	 */
	parts?: CSSGenericItem[];
	/**
	 * CSS properties
	 */
	properties?: CSSProperty[];
	/**
	 * CSS pseudo-classes
	 */
	"pseudo-classes"?: CSSPseudoClass[];
	/**
	 * CSS pseudo-elements
	 */
	"pseudo-elements"?: CSSPseudoElement[];
	[property: string]: unknown;
}

/**
 * Contains contributions to CSS namespace. It's property names represent symbol kinds, its
 * property values contain list of contributions of particular kind. There are predefined
 * kinds, which integrate directly with IDE - properties, classes, functions,
 * pseudo-elements, pseudo-classes and parts.
 */
export interface CSS {
	/**
	 * CSS classes
	 */
	classes?: CSSGenericItem[];
	/**
	 * CSS functions
	 */
	functions?: CSSGenericItem[];
	/**
	 * CSS parts
	 */
	parts?: CSSGenericItem[];
	/**
	 * CSS properties
	 */
	properties?: CSSProperty[];
	/**
	 * CSS pseudo-classes
	 */
	"pseudo-classes"?: CSSPseudoClass[];
	/**
	 * CSS pseudo-elements
	 */
	"pseudo-elements"?: CSSPseudoElement[];
	[property: string]: unknown;
}

/**
 * Specifies whether the symbol is deprecated. Deprecated symbol usage is discouraged, but
 * still supported. Value can be a boolean or a string message with explanation and
 * migration information.
 *
 * Specifies whether the symbol is experimental. Value can be a boolean or a string message
 * with explanation. Experimental symbols should be used with caution as the API might
 * change.
 *
 * Specifies whether the symbol is obsolete. Obsolete symbols are no longer supported. Value
 * can be a boolean or a string message with explanation and migration information.
 */
export type Deprecated = boolean | string;

/**
 * A reference to an element in Web-Types model.
 */
export type Reference = ReferenceWithProps | string;

export interface ReferenceWithProps {
	filter?: string;
	includeAbstract?: boolean;
	includeVirtual?: boolean;
	/**
	 * Override global name conversion rules for matching symbols under the path.
	 */
	"name-conversion"?: ReferenceNameConversion;
	path: string;
	[property: string]: unknown;
}

/**
 * Override global name conversion rules for matching symbols under the path.
 */
export interface ReferenceNameConversion {
	/**
	 * Override global canonical name conversion rule against which comparisons are made for the
	 * referenced symbols. When only rule name is specified, it applies to the symbols of the
	 * same kind as the last segment of the referenced path. Otherwise format of the property
	 * names is '{namespace}/{symbol kind}'. Supported by JetBrains IDEs since 2022.1.
	 */
	"canonical-names"?: CanonicalNames;
	/**
	 * Override global rules, by which referenced symbols should be matched against their
	 * canonical names. When only rule names are specified, they applies to the symbols of the
	 * same kind as the last segment of the referenced path. Otherwise format of the property
	 * names is '{namespace}/{symbol kind}'. Supported by JetBrains IDEs since 2022.1.
	 */
	"match-names"?: MatchNames;
	/**
	 * Override global rules, by which referenced symbol names should be proposed in auto
	 * completion. When only rule names are specified, they applies to the symbols of the same
	 * kind as the last segment of the referenced path. Otherwise format of the property names
	 * is '{namespace}/{symbol kind}'. Supported by JetBrains IDEs since 2022.1.
	 */
	"name-variants"?: MatchNames;
	[property: string]: unknown;
}

/**
 * Override global canonical name conversion rule against which comparisons are made for the
 * referenced symbols. When only rule name is specified, it applies to the symbols of the
 * same kind as the last segment of the referenced path. Otherwise format of the property
 * names is '{namespace}/{symbol kind}'. Supported by JetBrains IDEs since 2022.1.
 */
export type CanonicalNames = NameConversionRulesSingle | NameConverter;

/**
 * In many frameworks symbols can have multiple versions of a name. Specify canonical name
 * conversion rule for names of particular symbol kinds against which comparisons will be
 * made. Format of the 'canonical-names' property names is '{namespace}/{symbol kind}'. By
 * default symbol names in HTML namespace are converted to lower-case, and in CSS and JS
 * namespaces are left as-is. In case of name patterns, rules are applied to each part of
 * the pattern separately, so even if the symbol with pattern is in HTML namespace,
 * references to JS events will be case-sensitive.
 */
export type NameConversionRulesSingle = Record<string, unknown>;

export type NameConverter =
	| "as-is"
	| "PascalCase"
	| "camelCase"
	| "lowercase"
	| "UPPERCASE"
	| "kebab-case"
	| "snake_case";

/**
 * Override global rules, by which referenced symbols should be matched against their
 * canonical names. When only rule names are specified, they applies to the symbols of the
 * same kind as the last segment of the referenced path. Otherwise format of the property
 * names is '{namespace}/{symbol kind}'. Supported by JetBrains IDEs since 2022.1.
 *
 * Override global rules, by which referenced symbol names should be proposed in auto
 * completion. When only rule names are specified, they applies to the symbols of the same
 * kind as the last segment of the referenced path. Otherwise format of the property names
 * is '{namespace}/{symbol kind}'. Supported by JetBrains IDEs since 2022.1.
 */
export type MatchNames = NameConverter[] | NameConversionRulesMultiple;

/**
 * Provide an array of name conversions, in which particular symbol kinds should be matched
 * against canonical names of symbols. By default symbol names are converted using
 * canonical-names rule.
 *
 * Provide an array of name conversions, in which particular symbol kinds should be proposed
 * in auto completion. Format of the 'name-variants' property names is '{namespace}/{symbol
 * kind}'. All symbol kinds are by default provided as-is.
 */
export type NameConversionRulesMultiple = Record<string, unknown>;

export type NamePatternRoot = NamePattern | string;

export type NamePatternTemplate = NamePatternTemplate[] | NamePattern | string;

export interface NamePattern {
	delegate?: Reference;
	deprecated?: Deprecated;
	items?: ListReference;
	or?: NamePatternTemplate[];
	priority?: Priority;
	proximity?: number;
	repeat?: boolean;
	required?: boolean;
	template?: NamePatternTemplate[];
	unique?: boolean;
	"case-sensitive"?: boolean;
	regex?: string;
}

/**
 * A reference to an element in Web-Types model.
 */
export type ListReference = Reference[] | ReferenceWithProps | string;

export type Priority = "lowest" | "low" | "normal" | "high" | "highest";

/**
 * Allows to specify the source of the entity. For Vue.js component this may be for instance
 * a class.
 */
export interface Source {
	/**
	 * Path to the file, relative to the web-types JSON.
	 */
	file?: string;
	/**
	 * Offset in the file under which the source symbol, like class name, is located.
	 */
	offset?: number;
	/**
	 * Name of module, which exports the symbol. May be omitted, in which case it's assumed to
	 * be the name of the library.
	 */
	module?: string;
	/**
	 * Name of the exported symbol.
	 */
	symbol?: string;
}

/**
 * Kind of the symbol. Default is variable.
 */
export type JSSymbolKind = "Variable" | "Function" | "Namespace" | "Class" | "Interface" | "Enum" | "Alias" | "Module";

export type HTMLValueType = Type[] | TypeReference | string;

export type Type = TypeReference | string;

export interface TypeReference {
	/**
	 * Name of module, which exports the type. May be omitted, in which case it's assumed to be
	 * the name of the library.
	 */
	module?: string;
	/**
	 * Name of the symbol to import
	 */
	name: string;
}

export interface HTMLAttributeValue {
	default?: string;
	kind?: ValueKind;
	required?: boolean;
	type?: HTMLValueType;
}

export type ValueKind = "no-value" | "plain" | "expression";

/**
 * Deprecated vue-specific property - contribute Vue directives to
 * /contributions/html/vue-directives
 */
export interface DeprecatedHTMLAttributeVueArgument {
	description?: string;
	"doc-url"?: string;
	pattern?: NamePatternRoot;
	/**
	 * Whether directive requires an argument
	 */
	required?: boolean;
}

export interface DeprecatedHTMLAttributeVueModifier {
	description?: string;
	"doc-url"?: string;
	name: string;
	pattern?: NamePatternRoot;
}

/**
 * Deprecated, use top-level property.
 *
 * Markup language in which descriptions are formatted.
 */
export type DescriptionMarkup = "html" | "markdown" | "none";

/**
 * Deprecated, use top-level js-types-syntax property.
 *
 * Language in which JavaScript objects types are specified.
 */
export type TypesSyntax = "typescript";

/**
 * Contains contributions to JS namespace. It's property names represent symbol kinds, its
 * property values contain list of contributions of particular kind. There are 2 predefined
 * kinds, which integrate directly with IDE - properties and events, but only events can be
 * contributed globally.
 */
export interface JSGlobal {
	/**
	 * DOM events
	 */
	events?: GenericJSContributionElement[];
	/**
	 * Globally available symbols for JavaScript resolve. TypeScript resolve is not supported.
	 * Please note that these symbols will override any normally available global JavaScript
	 * symbols.
	 */
	symbols?: JSSymbol[];
	[property: string]: unknown;
}

/**
 * Provide configuration for the specified web framework. This is an advanced feature, which
 * is used to provide support for templating frameworks like Angular, Vue, Svelte, etc.
 */
export interface FrameworkConfig {
	/**
	 * In many frameworks symbols can have multiple versions of a name. Specify canonical name
	 * conversion rule for names of particular symbol kinds against which comparisons will be
	 * made. Format of the 'canonical-names' property names is '{namespace}/{symbol kind}'. By
	 * default symbol names in HTML namespace are converted to lower-case, and in CSS and JS
	 * namespaces are left as-is. In case of name patterns, rules are applied to each part of
	 * the pattern separately, so even if the symbol with pattern is in HTML namespace,
	 * references to JS events will be case-sensitive.
	 */
	"canonical-names"?: NameConversionRulesSingle;
	"disable-when"?: DisablementRules;
	"enable-when"?: EnablementRules;
	/**
	 * Provide an array of name conversions, in which particular symbol kinds should be matched
	 * against canonical names of symbols. By default symbol names are converted using
	 * canonical-names rule.
	 */
	"match-names"?: NameConversionRulesMultiple;
	/**
	 * Provide an array of name conversions, in which particular symbol kinds should be proposed
	 * in auto completion. Format of the 'name-variants' property names is '{namespace}/{symbol
	 * kind}'. All symbol kinds are by default provided as-is.
	 */
	"name-variants"?: NameConversionRulesMultiple;
}
