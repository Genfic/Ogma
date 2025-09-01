import { toCamelCaseKeys } from "es-toolkit";
import { createSignal } from "solid-js";

type InputType = "color" | "checkbox" | "number" | "text" | "email" | "password" | "url";

type NormalizedValue = string | number | boolean | null;

type FormData = Record<string, NormalizedValue>;

interface FormElement extends HTMLInputElement {
	name: string;
	type: InputType;
	value: string;
	checked: boolean;
}

interface NormalizedFormReturn<T extends FormData> {
	formData: Accessor<T>;
	handleInput: (name: keyof T, customValue?: NormalizedValue, raw?: boolean) => (event: Event) => void;
	handleSubmit: (event: SubmitEvent, rawValuesFor?: (keyof T)[]) => T;
	setFormData: Setter<T>;
}

// Normalization functions with proper typing
const normalizers: Record<InputType, (value: string, checked?: boolean) => NormalizedValue> = {
	color: (value: string): string => value.replace("#", ""),
	checkbox: (_, checked: boolean = false): boolean => checked,
	number: (value: string): number | null => (value === "" ? null : Number(value)),
	text: (value: string): string => value,
	email: (value: string): string => value.trim().toLowerCase(),
	password: (value: string): string => value,
	url: (value: string): string => value.trim(),
};

export function createNormalizedForm<T extends FormData>(initialData: T = {} as T): NormalizedFormReturn<T> {
	const [formData, setFormData] = createSignal<T>(initialData);

	const normalizeValue = (input: FormElement): NormalizedValue => {
		const { type, value, checked } = input;

		const normalizer = normalizers[type];
		if (normalizer) {
			return normalizer(value, checked);
		}

		return value;
	};

	const handleInput = (name: keyof T, customValue?: NormalizedValue, raw?: boolean) => (event: Event) => {
		const input = event.target as FormElement;
		if (!input.name) return;

		const normalizedValue = customValue ? customValue : raw ? input.value : normalizeValue(input);

		setFormData((prev) => ({
			...prev,
			[name]: normalizedValue,
		}));
	};

	const handleSubmit = (event: SubmitEvent, rawValuesFor?: (keyof T)[]): T => {
		event.preventDefault();
		const formElement = event.target as HTMLFormElement;
		const normalized = {} as T;

		// Normalize all form data
		const elements = Array.from(formElement.elements) as FormElement[];
		elements.forEach((input) => {
			if (input.name) {
				normalized[input.name as keyof T] = normalizeValue(input) as T[keyof T];
			}
		});

		return toCamelCaseKeys(normalized) as T;
	};

	return {
		formData,
		handleInput,
		handleSubmit,
		setFormData,
	};
}
