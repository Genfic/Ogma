/*!
 * Wellidate 2.2.3
 *
 * Copyright © NonFactors
 *
 * Licensed under the terms of the MIT License
 * http://www.opensource.org/licenses/mit-license.php
 */
(function (global, factory) {
    if (typeof define === "function" && define.amd) {
        define(["exports"], factory);
    } else if (typeof module === "object" && module.exports) {
        factory(module.exports);
    } else {
        factory(global);
    }
}(this, exports => {
    class WellidateValidatable {
        constructor(wellidate, group) {
            const validatable = this;

            validatable.rules = {};
            validatable.bindings = [];
            validatable.isValid = true;
            validatable.isDirty = false;
            validatable.elements = group;
            validatable.element = group[0];
            validatable.wellidate = wellidate;

            validatable.buildErrorContainers();
            validatable.buildInputRules();
            validatable.buildDataRules();
        }

        validate() {
            const pending = [];
            const validatable = this;

            validatable.isValid = true;

            for (const method of Object.keys(validatable.rules)) {
                const rule = validatable.rules[method];

                if (rule.isEnabled()) {
                    const isValid = rule.isValid(validatable);

                    if (!isValid) {
                        validatable.isValid = false;
                        validatable.error(method);

                        break;
                    } else if (typeof isValid !== "boolean") {
                        pending.push(isValid);
                        validatable.pending();

                        isValid.then(result => {
                            if (validatable.isValid && !result) {
                                validatable.isValid = false;
                                validatable.error(method);
                            }
                        });
                    }
                }
            }

            if (pending.length) {
                Promise.all(pending).then(isValid => {
                    if (validatable.isValid && isValid.every(Boolean)) {
                        validatable.success();
                    }
                });
            } else if (validatable.isValid) {
                validatable.success();
            }

            return validatable.isValid;
        }

        reset(message) {
            const validatable = this;
            const wellidate = validatable.wellidate;

            validatable.isDirty = false;
            validatable.element.setCustomValidity("");

            for (const element of validatable.elements) {
                element.classList.remove(wellidate.inputErrorClass);
                element.classList.remove(wellidate.inputValidClass);
                element.classList.remove(wellidate.inputPendingClass);
            }

            for (const container of validatable.errorContainers) {
                container.classList.remove(wellidate.fieldPendingClass);
                container.classList.remove(wellidate.fieldErrorClass);
                container.classList.remove(wellidate.fieldValidClass);
                container.innerHTML = message || "";
            }

            validatable.element.dispatchEvent(new CustomEvent("wellidate-reset", {
                detail: { validatable },
                bubbles: true
            }));
        }
        pending(message) {
            const wellidate = this.wellidate;

            for (const element of this.elements) {
                element.classList.add(wellidate.inputPendingClass);
                element.classList.remove(wellidate.inputValidClass);
                element.classList.remove(wellidate.inputErrorClass);
            }

            for (const container of this.errorContainers) {
                container.classList.remove(wellidate.fieldErrorClass);
                container.classList.remove(wellidate.fieldValidClass);
                container.classList.add(wellidate.fieldPendingClass);
                container.innerHTML = message || "";
            }

            this.element.dispatchEvent(new CustomEvent("wellidate-pending", {
                detail: { validatable: this },
                bubbles: true
            }));
        }
        success(message) {
            const validatable = this;
            const wellidate = validatable.wellidate;

            validatable.element.setCustomValidity("");

            for (const element of validatable.elements) {
                element.classList.add(wellidate.inputValidClass);
                element.classList.remove(wellidate.inputErrorClass);
                element.classList.remove(wellidate.inputPendingClass);
            }

            for (const container of validatable.errorContainers) {
                container.classList.remove(wellidate.fieldPendingClass);
                container.classList.remove(wellidate.fieldErrorClass);
                container.classList.add(wellidate.fieldValidClass);
                container.innerHTML = message || "";
            }

            validatable.element.dispatchEvent(new CustomEvent("wellidate-success", {
                detail: { validatable },
                bubbles: true
            }));
        }
        error(method, message) {
            const validatable = this;
            const wellidate = validatable.wellidate;
            const rule = method ? validatable.rules[method] : null;
            const formattedMessage = message || rule.formatMessage();

            validatable.isDirty = true;
            validatable.element.setCustomValidity(formattedMessage);

            for (const element of validatable.elements) {
                element.classList.add(wellidate.inputErrorClass);
                element.classList.remove(wellidate.inputValidClass);
                element.classList.remove(wellidate.inputPendingClass);
            }

            for (const container of validatable.errorContainers) {
                container.classList.remove(wellidate.fieldPendingClass);
                container.classList.remove(wellidate.fieldValidClass);
                container.classList.add(wellidate.fieldErrorClass);
                container.innerHTML = formattedMessage;
            }

            validatable.element.dispatchEvent(new CustomEvent("wellidate-error", {
                detail: {
                    message: formattedMessage,
                    validatable: validatable,
                    method: method
                },
                bubbles: true
            }));
        }

        bind() {
            const validatable = this;
            const wellidate = this.wellidate;
            const input = validatable.element;
            const event = input.tagName === "SELECT" || input.type === "hidden" ? "change" : "input";

            function changeEvent() {
                if (this.type === "hidden" || validatable.isDirty) {
                    validatable.validate();
                }
            }
            function blurEvent() {
                if (validatable.isDirty || this.value.length) {
                    validatable.isDirty = !validatable.validate();
                }
            }
            function focusEvent() {
                if (wellidate.focusCleanup) {
                    validatable.reset();
                }

                wellidate.lastActive = this;
            }

            for (const element of validatable.elements) {
                element.addEventListener("blur", blurEvent);
                element.addEventListener(event, changeEvent);
                element.addEventListener("focus", focusEvent);
            }

            validatable.bindings.push(blurEvent, changeEvent, focusEvent);
        }
        unbind() {
            for (const binding of this.bindings) {
                for (const element of this.elements) {
                    element.removeEventListener("blur", binding);
                    element.removeEventListener("focus", binding);
                    element.removeEventListener("input", binding);
                    element.removeEventListener("change", binding);
                }
            }
        }

        buildErrorContainers() {
            let name = this.element.name;

            if (name) {
                name = name.replace(/(["\]\\])/g, "\\$1");

                this.errorContainers = Array.from(document.querySelectorAll(`[data-valmsg-for="${name}"]`));
            } else {
                this.errorContainers = [];
            }
        }
        buildInputRules() {
            const validatable = this;
            const rules = validatable.rules;
            const element = validatable.element;
            const defaultRule = Wellidate.default.rule;
            const defaultRules = Wellidate.default.rules;

            if (element.required && defaultRules.required) {
                rules.required = Object.assign({}, defaultRule, defaultRules.required, { element });
            }

            if (element.type === "email" && defaultRules.email) {
                rules.email = Object.assign({}, defaultRule, defaultRules.email, { element });
            }

            if (element.accept && defaultRules.accept) {
                rules.accept = Object.assign({}, defaultRule, defaultRules.accept, {
                    types: element.accept,
                    element: element
                });
            }

            if (element.getAttribute("minlength") && defaultRules.length) {
                rules.length = Object.assign({}, defaultRule, defaultRules.length, rules.length, {
                    min: element.getAttribute("minlength"),
                    element: element
                });
            }

            if (element.getAttribute("maxlength") && defaultRules.length) {
                rules.length = Object.assign({}, defaultRule, defaultRules.length, rules.length, {
                    max: element.getAttribute("maxlength"),
                    element: element
                });
            }

            if (element.min && defaultRules.range) {
                rules.range = Object.assign({}, defaultRule, defaultRules.range, rules.range, {
                    min: element.min,
                    element: element
                });
            }

            if (element.max && defaultRules.range) {
                rules.range = Object.assign({}, defaultRule, defaultRules.range, rules.range, {
                    max: element.max,
                    element: element
                });
            }

            if (element.step && defaultRules.step) {
                rules.step = Object.assign({}, defaultRule, defaultRules.step, {
                    value: element.step,
                    element: element
                });
            }

            if (element.pattern && defaultRules.regex) {
                rules.regex = Object.assign({}, defaultRule, defaultRules.regex, {
                    pattern: element.pattern,
                    title: element.title,
                    element: element
                });
            }
        }
        buildDataRules() {
            const element = this.element;
            const defaultRule = Wellidate.default.rule;
            const attributes = Array.from(element.attributes).filter(attribute => /^data-val-\w+$/.test(attribute.name));

            for (const attribute of attributes) {
                const prefix = attribute.name;
                const method = prefix.substring(9);
                const rule = this.rules[method] || Wellidate.default.rules[method];

                if (rule) {
                    const dataRule = {
                        message: attribute.value || rule.message,
                        isDataMessage: Boolean(attribute.value)
                    };

                    for (const parameter of Array.from(element.attributes)) {
                        if (parameter.name.startsWith(`${prefix}-`)) {
                            dataRule[parameter.name.substring(prefix.length + 1)] = parameter.value;
                        }
                    }

                    this.rules[method] = Object.assign({}, defaultRule, rule, dataRule, { element });
                }
            }
        }
    }

    class Wellidate {
        constructor(container, options = {}) {
            const wellidate = this;

            if (container.dataset.valId) {
                return Wellidate.instances[parseInt(container.dataset.valId)].set(options);
            }

            wellidate.wasValidatedClass = Wellidate.default.classes.wasValidated;
            wellidate.inputPendingClass = Wellidate.default.classes.inputPending;
            wellidate.fieldPendingClass = Wellidate.default.classes.fieldPending;
            wellidate.inputErrorClass = Wellidate.default.classes.inputError;
            wellidate.inputValidClass = Wellidate.default.classes.inputValid;
            wellidate.fieldErrorClass = Wellidate.default.classes.fieldError;
            wellidate.fieldValidClass = Wellidate.default.classes.fieldValid;
            container.dataset.valId = Wellidate.instances.length.toString();
            wellidate.summary = wellidate.extend(Wellidate.default.summary);
            wellidate.focusCleanup = Wellidate.default.focusCleanup;
            wellidate.focusInvalid = Wellidate.default.focusInvalid;
            wellidate.excludes = Wellidate.default.excludes.slice();
            wellidate.include = Wellidate.default.include;
            wellidate.container = container;
            wellidate.validatables = [];

            if (container.tagName === "FORM") {
                container.noValidate = true;
            }

            wellidate.set(options);
            wellidate.bind();

            Wellidate.instances.push(wellidate);
        }

        set(options) {
            const wellidate = this;

            wellidate.setOption("include", options.include);
            wellidate.setOption("summary", options.summary);
            wellidate.setOption("excludes", options.excludes);
            wellidate.setOption("focusCleanup", options.focusCleanup);
            wellidate.setOption("focusInvalid", options.focusInvalid);
            wellidate.setOption("submitHandler", options.submitHandler);
            wellidate.setOption("fieldValidClass", options.fieldValidClass);
            wellidate.setOption("fieldErrorClass", options.fieldErrorClass);
            wellidate.setOption("inputValidClass", options.inputValidClass);
            wellidate.setOption("inputErrorClass", options.inputErrorClass);
            wellidate.setOption("fieldPendingClass", options.fieldPendingClass);
            wellidate.setOption("inputPendingClass", options.inputPendingClass);
            wellidate.setOption("wasValidatedClass", options.wasValidatedClass);

            wellidate.rebuild();

            for (const selector of Object.keys(options.rules || {})) {
                for (const validatable of wellidate.filterValidatables(selector)) {
                    const element = validatable.element;

                    for (const method of Object.keys(options.rules[selector])) {
                        const defaultRule = Wellidate.default.rule;
                        const newRule = options.rules[selector][method];
                        const methodRule = validatable.rules[method] || Wellidate.default.rules[method] || {};

                        validatable.rules[method] = wellidate.extend(defaultRule, methodRule, newRule, { element });
                    }
                }
            }

            return wellidate;
        }

        rebuild() {
            const wellidate = this;
            const validatables = [];

            wellidate.validatables.forEach(validatable => validatable.unbind());

            if (wellidate.container.matches(wellidate.include)) {
                const group = wellidate.buildGroupElements(wellidate.container);
                const current = wellidate.validatables.find(validatable => validatable.element === group[0]);

                validatables.push(current || new WellidateValidatable(wellidate, group));
                validatables[validatables.length - 1].bind();
            } else {
                for (const element of wellidate.container.querySelectorAll(wellidate.include)) {
                    const group = wellidate.buildGroupElements(element);

                    if (element === group[0]) {
                        const current = wellidate.validatables.find(validatable => validatable.element === element);

                        validatables.push(current || new WellidateValidatable(wellidate, group));
                        validatables[validatables.length - 1].bind();
                    }
                }
            }

            wellidate.validatables = validatables;
        }
        form(...filter) {
            return !this.validateAndApply(...filter).invalid.length;
        }
        isValid(...filter) {
            for (const validatable of this.filterValidatables(...filter)) {
                for (const method of Object.keys(validatable.rules)) {
                    const rule = validatable.rules[method];

                    if (rule.isEnabled()) {
                        const isValid = rule.isValid(validatable);

                        if (!isValid) {
                            validatable.isValid = false;

                            return false;
                        } else if (typeof isValid !== "boolean") {
                            isValid.then(result => {
                                if (!result) {
                                    validatable.isValid = false;
                                }
                            });
                        }
                    }
                }

                validatable.isValid = true;
            }

            return true;
        }
        apply(results) {
            for (const selector of Object.keys(results)) {
                for (const validatable of this.filterValidatables(selector)) {
                    const result = results[selector];

                    if (typeof result.error !== "undefined") {
                        validatable.error(null, result.error);
                    } else if (typeof result.success !== "undefined") {
                        validatable.success(result.success);
                    } else if (typeof result.reset !== "undefined") {
                        validatable.reset(result.reset);
                    }
                }
            }
        }
        validate(...filter) {
            const results = {
                isValid: true,
                pending: [],
                invalid: [],
                valid: []
            };

            for (const validatable of this.filterValidatables(...filter)) {
                const rules = [];

                validatable.isValid = true;

                for (const method of Object.keys(validatable.rules)) {
                    const rule = validatable.rules[method];
                    const isValid = !rule.isEnabled() || rule.isValid(validatable);

                    if (!isValid) {
                        results.invalid.push({
                            message: rule.formatMessage(),
                            validatable: validatable,
                            method: method
                        });

                        validatable.isValid = false;
                        results.isValid = false;

                        break;
                    } else if (typeof isValid !== "boolean") {
                        isValid.then(result => {
                            validatable.isValid = validatable.isValid && result;
                        });

                        rules.push({ method: method, promise: isValid });

                        if (!results.pending.some(pending => pending.validatable === validatable)) {
                            results.pending.push({
                                validatable: validatable,
                                rules: rules
                            });
                        }
                    }
                }

                if (validatable.isValid && !rules.length) {
                    results.valid.push({ validatable });
                }
            }

            return results;
        }

        reset() {
            const wellidate = this;

            wellidate.summary.reset();

            wellidate.container.classList.remove(wellidate.wasValidatedClass);

            for (const validatable of wellidate.validatables) {
                validatable.reset();
            }
        }

        extend(...args) {
            const options = {};

            for (const arg of args) {
                for (const key of Object.keys(arg)) {
                    if (Object.prototype.toString.call(options[key]) === "[object Object]") {
                        options[key] = this.extend(options[key], arg[key]);
                    } else {
                        options[key] = arg[key];
                    }
                }
            }

            return options;
        }
        setOption(option, value) {
            const wellidate = this;

            if (typeof value !== "undefined") {
                if (Object.prototype.toString.call(value) === "[object Object]") {
                    wellidate[option] = wellidate.extend(wellidate[option], value);
                } else {
                    wellidate[option] = value;
                }
            }
        }

        buildGroupElements(group) {
            if (group.name) {
                const name = group.name.replace(/(["\]\\])/g, "\\$1");

                return Array.from(document.querySelectorAll(`[name="${name}"]`));
            }

            return [group];
        }

        focus(errors) {
            if (errors.length) {
                let invalid = errors[0];

                for (let i = 1; i < errors.length; i++) {
                    if (this.lastActive === errors[i].element) {
                        invalid = errors[i];

                        break;
                    } else if (invalid.element.compareDocumentPosition(errors[i].element) === 2) {
                        invalid = errors[i];
                    }
                }

                this.lastActive = invalid.element;

                if (this.focusCleanup) {
                    invalid.reset();
                }

                invalid.element.focus();
            }
        }
        isExcluded(element) {
            for (const exclude of this.excludes) {
                if (element.matches(exclude)) {
                    return true;
                }
            }

            return false;
        }
        validateAndApply(...filter) {
            const wellidate = this;
            const results = wellidate.validate(...filter);

            for (const valid of results.valid) {
                valid.validatable.success();
            }

            for (const pending of results.pending) {
                pending.validatable.pending();

                Promise.all(pending.rules.map(rule => rule.promise)).then(promises => {
                    const error = promises.findIndex(isValid => !isValid);

                    if (error >= 0) {
                        pending.validatable.error(pending.rules[error].method);
                    } else {
                        pending.validatable.success();
                    }
                });
            }

            for (const invalid of results.invalid) {
                invalid.validatable.error(invalid.method);
            }

            wellidate.summary.show(results);

            if (wellidate.focusInvalid) {
                wellidate.focus(results.invalid.map(invalid => invalid.validatable));
            }

            wellidate.container.classList.add(wellidate.wasValidatedClass);

            return results;
        }
        filterValidatables(...filter) {
            return this.validatables.filter(validatable => {
                for (const filterId of filter) {
                    if (validatable.element.matches(filterId)) {
                        return true;
                    }
                }

                return !filter.length;
            }).filter(validatable => !this.isExcluded(validatable.element));
        }

        bind() {
            const wellidate = this;

            if (wellidate.container.tagName === "FORM") {
                wellidate.container.addEventListener("submit", function (e) {
                    const results = wellidate.validateAndApply();

                    if (results.invalid.length) {
                        e.preventDefault();

                        this.dispatchEvent(new CustomEvent("wellidate-invalid", {
                            detail: { wellidate },
                            bubbles: true
                        }));
                    } else {
                        this.dispatchEvent(new CustomEvent("wellidate-valid", {
                            detail: { wellidate },
                            bubbles: true
                        }));

                        if (wellidate.submitHandler) {
                            e.preventDefault();

                            wellidate.submitHandler(e, results);
                        }
                    }
                });

                wellidate.container.addEventListener("reset", () => {
                    wellidate.reset();
                });
            }
        }
    }

    Wellidate.default = {
        focusInvalid: true,
        focusCleanup: false,
        include: "input,textarea,select",
        summary: {
            container: "[data-valmsg-summary=true]",
            show(result) {
                if (this.container) {
                    const summary = document.querySelector(this.container);

                    if (summary) {
                        summary.innerHTML = "";

                        if (result.isValid) {
                            summary.classList.add("validation-summary-valid");
                            summary.classList.remove("validation-summary-errors");
                        } else {
                            summary.classList.add("validation-summary-errors");
                            summary.classList.remove("validation-summary-valid");

                            const list = document.createElement("ul");

                            for (const invalid of result.invalid) {
                                const item = document.createElement("li");

                                item.innerHTML = invalid.message;

                                list.appendChild(item);
                            }

                            summary.appendChild(list);
                        }

                        for (const pending of result.pending) {
                            Promise.all(pending.rules.map(rule => rule.promise)).then(results => {
                                const error = results.findIndex(isValid => !isValid);

                                if (error >= 0) {
                                    this.append(pending.validatable.rules[pending.rules[error].method].formatMessage());
                                }
                            });
                        }
                    }
                }
            },
            append(error) {
                if (this.container) {
                    const summary = document.querySelector(this.container);

                    if (summary) {
                        summary.classList.add("validation-summary-errors");
                        summary.classList.remove("validation-summary-valid");

                        const list = document.createElement("ul");
                        const item = document.createElement("li");

                        item.innerHTML = error;
                        list.appendChild(item);

                        summary.appendChild(list);
                    }
                }
            },
            reset() {
                this.show({
                    isValid: true,
                    pending: [],
                    invalid: [],
                    valid: []
                });
            }
        },
        classes: {
            inputPending: "input-validation-pending",
            inputError: "input-validation-error",
            inputValid: "input-validation-valid",
            fieldPending: "input-validation-pending",
            fieldError: "field-validation-error",
            fieldValid: "field-validation-valid",
            wasValidated: "was-validated"
        },
        excludes: [
            "input[type=button]",
            "input[type=submit]",
            "input[type=image]",
            "input[type=reset]",
            ":disabled"
        ],
        rule: {
            trim: true,
            message: "This field is not valid.",
            isValid() {
                return false;
            },
            isEnabled() {
                return true;
            },
            formatMessage() {
                return this.message;
            },
            normalizeValue(element) {
                const input = element || this.element;
                let value = input.value;

                if (input.tagName === "SELECT" && input.multiple) {
                    return Array.from(input.options).filter(option => option.selected).length.toString();
                } else if (input.type === "radio") {
                    if (input.name) {
                        const name = input.name.replace(/(["\]\\])/g, "\\$1");
                        const checked = document.querySelector(`input[name="${name}"]:checked`);

                        value = checked ? checked.value : "";
                    } else {
                        value = input.checked ? value : "";
                    }
                } else if (input.type === "file") {
                    if (value.lastIndexOf("\\") >= 0) {
                        value = value.substring(value.lastIndexOf("\\") + 1);
                    } else if (value.lastIndexOf("/") >= 0) {
                        value = value.substring(value.lastIndexOf("/") + 1);
                    }
                }

                return this.trim ? value.trim() : value;
            }
        },
        rules: {
            required: {
                message: "This field is required.",
                isValid() {
                    return Boolean(this.normalizeValue());
                }
            },
            equalto: {
                message: "Please enter the same value again.",
                isValid() {
                    const id = `${this.element.id.split('_')[0]}_${this.other.split('.')[1]}`;
                    const other = document.getElementById(id);

                    return other && this.normalizeValue() === this.normalizeValue(other);
                }
            },
            length: {
                message: "Please enter a value between {0} and {1} characters long.",
                isValid() {
                    const length = this;
                    const value = length.normalizeValue();

                    return (length.min == null || length.min <= value.length) && (value.length <= length.max || length.max == null);
                },
                formatMessage() {
                    const length = this;

                    if (length.min != null && length.max == null && !length.isDataMessage) {
                        return Wellidate.default.rules.minlength.message.replace("{0}", length.min);
                    } else if (length.min == null && length.max != null && !length.isDataMessage) {
                        return Wellidate.default.rules.maxlength.message.replace("{0}", length.max);
                    }

                    return length.message.replace("{0}", length.min).replace("{1}", length.max);
                }
            },
            minlength: {
                message: "Please enter at least {0} characters.",
                isValid() {
                    return this.min <= this.normalizeValue().length;
                },
                formatMessage() {
                    return this.message.replace("{0}", this.min);
                }
            },
            maxlength: {
                message: "Please enter no more than {0} characters.",
                isValid() {
                    return this.normalizeValue().length <= this.max;
                },
                formatMessage() {
                    return this.message.replace("{0}", this.max);
                }
            },
            email: {
                message: "Please enter a valid email address.",
                isValid() {
                    return /^$|^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/.test(this.normalizeValue());
                }
            },
            integer: {
                message: "Please enter a valid integer value.",
                isValid() {
                    return /^$|^[+-]?\d+$/.test(this.normalizeValue());
                }
            },
            number: {
                groupSeparator: ",",
                decimalSeparator: ".",
                message: "Please enter a valid number.",
                scaleMessage: "Please enter a value with no more than {0} fractional digits",
                precisionMessage: "Please enter a value using no more than {0} significant digits",
                isValid() {
                    const number = this;
                    const scale = parseInt(number.scale);
                    const value = number.normalizeValue();
                    const precision = parseInt(number.precision);
                    const parts = value.split(this.decimalSeparator);
                    let isValid = new RegExp(`^$|^[+-]?(\\d*|\\d[0-9${this.groupSeparator}]*)[${this.decimalSeparator}]?\\d*$`).test(value);

                    if (isValid && value && precision > 0) {
                        const digits = number.digits(parts[0].replace(new RegExp(`^[-+${this.groupSeparator}0]+`), ""));

                        number.isValidPrecision = digits <= precision - (scale || 0);
                        isValid = number.isValidPrecision;
                    } else {
                        number.isValidPrecision = true;
                    }

                    if (isValid && parts[1] && scale >= 0) {
                        const digits = number.digits(parts[1].replace(/0+$/, ""));

                        number.isValidScale = digits <= scale;
                        isValid = number.isValidScale;
                    } else {
                        number.isValidScale = true;
                    }

                    return isValid;
                },
                digits(value) {
                    return value.split("").filter(e => !isNaN(parseInt(e))).length;
                },
                formatMessage() {
                    const number = this;

                    if (number.isValidPrecision === false && !number.isDataMessage) {
                        return number.precisionMessage.replace("{0}", parseInt(number.precision) - (parseInt(number.scale) || 0));
                    } else if (number.isValidScale === false && !number.isDataMessage) {
                        return number.scaleMessage.replace("{0}", parseInt(number.scale) || 0);
                    }

                    return number.message;
                }
            },
            digits: {
                message: "Please enter only digits.",
                isValid() {
                    return /^\d*$/.test(this.normalizeValue());
                }
            },
            date: {
                message: "Please enter a valid date.",
                isValid() {
                    return /^$|^\d{4}-(0?[1-9]|1[012])-(0?[1-9]|[12][0-9]|3[01])$/.test(this.normalizeValue());
                }
            },
            range: {
                message: "Please enter a value between {0} and {1}.",
                isValid() {
                    const min = parseFloat(this.min);
                    const max = parseFloat(this.max);
                    const value = this.normalizeValue();

                    return !value || (isNaN(min) || min <= value) && (value <= max || isNaN(max));
                },
                formatMessage() {
                    const range = this;

                    if (range.min != null && range.max == null && !range.isDataMessage) {
                        return Wellidate.default.rules.min.message.replace("{0}", range.min);
                    } else if (range.min == null && range.max != null && !range.isDataMessage) {
                        return Wellidate.default.rules.max.message.replace("{0}", range.max);
                    }

                    return range.message.replace("{0}", range.min).replace("{1}", range.max);
                }
            },
            min: {
                message: "Please enter a value greater than or equal to {0}.",
                isValid() {
                    const value = this.normalizeValue();

                    return !value || parseFloat(this.value) <= value;
                },
                formatMessage() {
                    return this.message.replace("{0}", this.value);
                }
            },
            max: {
                message: "Please enter a value less than or equal to {0}.",
                isValid() {
                    const value = this.normalizeValue();

                    return !value || value <= parseFloat(this.value);
                },
                formatMessage() {
                    return this.message.replace("{0}", this.value);
                }
            },
            greater: {
                message: "Please enter a value greater than {0}.",
                isValid() {
                    const value = this.normalizeValue();

                    return !value || parseFloat(this.than) < value;
                },
                formatMessage() {
                    return this.message.replace("{0}", this.than);
                }
            },
            lower: {
                message: "Please enter a value lower than {0}.",
                isValid() {
                    const value = this.normalizeValue();

                    return !value || value < parseFloat(this.than);
                },
                formatMessage() {
                    return this.message.replace("{0}", this.than);
                }
            },
            step: {
                message: "Please enter a multiple of {0}.",
                isValid() {
                    const value = this.normalizeValue();

                    return !value || value % parseInt(this.value) === 0;
                },
                formatMessage() {
                    return this.message.replace("{0}", this.value);
                }
            },
            filesize: {
                page: 1024,
                message: "File size should not exceed {0} MB.",
                isValid() {
                    const size = Array.from(this.element.files).reduce((total, file) => total + file.size, 0);

                    return size <= this.max || this.max == null;
                },
                formatMessage() {
                    const filesize = this;
                    const mb = (filesize.max / filesize.page / filesize.page).toFixed(2);

                    return filesize.message.replace("{0}", mb.replace(/[.|0]*$/, ""));
                }
            },
            accept: {
                message: "Please select files in correct format.",
                isValid() {
                    const filter = this.types.split(",").map(type => type.trim());

                    const correct = Array.from(this.element.files).filter(file => {
                        const extension = file.name.split(".").pop();

                        for (const type of filter) {
                            if (type.startsWith(".")) {
                                if (file.name !== extension && `.${extension}` === type) {
                                    return true;
                                }
                            } else if (type.endsWith("/*")) {
                                if (file.type.startsWith(type.replace(/\*$/, ""))) {
                                    return true;
                                }
                            } else if (file.type === type) {
                                return true;
                            }
                        }

                        return !filter.length;
                    });

                    return this.element.files.length === correct.length;
                }
            },
            regex: {
                message: "Please enter value in a valid format. {0}",
                isValid() {
                    const value = this.normalizeValue();

                    return !value || !this.pattern || new RegExp(this.pattern).test(value);
                },
                formatMessage() {
                    return this.message.replace("{0}", this.title || "");
                }
            },
            remote: {
                type: "get",
                message: "Please fix this field.",
                isValid(validatable) {
                    const remote = this;

                    if (remote.controller) {
                        remote.controller.abort();
                    }

                    clearTimeout(remote.start);

                    return new Promise((resolve, reject) => {
                        remote.start = setTimeout(() => {
                            if (validatable.isValid) {
                                remote.controller = new AbortController();

                                remote.prepare(validatable)
                                    .then(response => response.ok ? response.text() : "")
                                    .then(response => {
                                        if (response) {
                                            const result = JSON.parse(response);

                                            remote.message = result.message || remote.message;

                                            resolve(result.isValid !== false);
                                        } else {
                                            resolve(true);
                                        }
                                    }).catch(reason => {
                                    if (reason.name === "AbortError") {
                                        resolve(true);
                                    }

                                    reject(reason);
                                });
                            } else {
                                resolve(true);
                            }
                        }, 1);
                    });
                },
                buildUrl() {
                    const remote = this;
                    const url = new URL(remote.url, location.href);
                    const fields = (remote.additionalFields || "").split(",").filter(Boolean);

                    for (const field of fields) {
                        const element = document.querySelector(field);

                        url.searchParams.append(element.name, remote.normalizeValue(element) || "");
                    }

                    url.searchParams.append(remote.element.name, remote.normalizeValue() || "");

                    return url.href;
                },
                prepare() {
                    return fetch(this.buildUrl(), {
                        method: this.type,
                        signal: this.controller.signal,
                        headers: { "X-Requested-With": "XMLHttpRequest" }
                    });
                }
            }
        }
    };
    Wellidate.instances = [];

    exports.Wellidate = Wellidate;
    exports.WellidateValidatable = WellidateValidatable;
}));
