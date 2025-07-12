import { createSignal } from "solid-js";

type InputType = 'color' | 'checkbox' | 'number' | 'text' | 'email' | 'password' | 'url';

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
	handleInput: (event: Event) => void;
	handleSubmit: (event: SubmitEvent) => T;
	setFormData: Setter<T>;
}

// Normalization functions with proper typing
const normalizers: Record<InputType, (value: string, checked?: boolean) => NormalizedValue> = {
	color: (value: string): string => value.replace('#', ''),
	checkbox: (_, checked: boolean = false): boolean => checked,
	number: (value: string): number | null => value === '' ? null : Number(value),
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

	const handleInput = (event: Event): void => {
		const input = event.target as FormElement;
		if (!input.name) return;

		const normalizedValue = normalizeValue(input);

		setFormData(prev => ({
			...prev,
			[input.name]: normalizedValue
		} as T));
	};

	const handleSubmit = (event: SubmitEvent): T => {
		event.preventDefault();
		const formElement = event.target as HTMLFormElement;
		const normalized = {} as T;

		// Normalize all form data
		const elements = Array.from(formElement.elements) as FormElement[];
		elements.forEach(input => {
			if (input.name) {
				normalized[input.name as keyof T] = normalizeValue(input) as T[keyof T];
			}
		});

		return normalized;
	};

	return {
		formData,
		handleInput,
		handleSubmit,
		setFormData
	};
}