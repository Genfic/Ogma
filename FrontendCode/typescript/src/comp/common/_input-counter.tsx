import { templateReplace } from "@h/string-helpers";
import type { JSX } from "solid-js";
import { type Component, mergeProps, Show } from "solid-js";

interface InputCounterProps {
	label: string;
	max: number;
	min?: number;
	value?: string;
	onInput?: (event: InputEvent) => void;
	desc?: string;
	validateMsg?: string;
}

export const InputCounter: Component<InputCounterProps> = (props) => {
	const merged = mergeProps(
		{
			min: 0,
			value: "",
			desc: null,
			validateMsg: "{0} must be between {2} and {1} characters.",
			onInput: () => {},
		},
		props,
	);

	let text = $signal(merged.value);

	$(() => {
		text = merged.value;
	});

	const name = $(merged.label.replace(/\s+/g, ""));
	const charCount = $(text.length);
	const isValid = $(charCount >= merged.min && charCount <= merged.max);

	const validationString = $(
		templateReplace(merged.validateMsg, {
			"{0}": merged.label.toLowerCase(),
			"{1}": `${merged.max}`,
			"{2}": `${merged.min}`,
		}),
	);

	const handleInput: JSX.EventHandler<HTMLInputElement, InputEvent> = (event) => {
		text = event.currentTarget.value;
		merged.onInput(event);
	};

	return (
		<div class="o-form-group">
			<label for={name}>{merged.label.replace(/([A-Z])/g, " $1")}</label>

			<Show when={merged.desc}>
				<p class="desc">{merged.desc}</p>
			</Show>

			<input
				name={name}
				id={name}
				type="text"
				class="o-form-control active-border"
				value={text}
				oninput={handleInput}
			/>

			<div class="counter" classList={{ invalid: !isValid }}>
				<div class="o-progress-bar" style={{ width: `${Math.min(100, (100 * charCount) / merged.max)}%` }} />
				<span>
					{charCount.toLocaleString()}/{merged.max.toLocaleString()}
				</span>
			</div>

			<span>{validationString}</span>
		</div>
	);
};
