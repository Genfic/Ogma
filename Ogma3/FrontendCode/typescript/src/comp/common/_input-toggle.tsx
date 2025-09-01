import { type Component, createUniqueId, type JSX, mergeProps, Show } from "solid-js";

interface InputToggleProps {
	label: string;
	desc?: string;
	value?: boolean;
	onChange?: (event: Event) => void;
}

const id = createUniqueId();

export const InputToggle: Component<InputToggleProps> = (props) => {
	const merged = mergeProps(
		{
			desc: null,
			value: false,
			onChange: () => {},
		},
		props,
	);

	let isChecked = $(merged.value);

	const name = $(merged.label.replace(/\s+/g, ""));

	const handleChange: JSX.EventHandler<HTMLInputElement, Event> = (event) => {
		isChecked = event.currentTarget.checked;
		merged.onChange(event);
	};

	return (
		<div class="o-form-group keep-size">
			<legend>{merged.label.replace(/([A-Z])/g, " $1")}</legend>
			<Show when={merged.desc}>
				<p class="desc">{merged.desc}</p>
			</Show>

			<div class="toggle-input">
				<input type="checkbox" name={name} id={id + name} checked={isChecked} onchange={handleChange} />
				<label for={id + name}>
					<span class="toggle">
						<span class="dot" />
					</span>
					{isChecked ? "On" : "Off"}
				</label>
			</div>
		</div>
	);
};
