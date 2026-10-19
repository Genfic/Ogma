import { type Component, createUniqueId, mergeProps, Show } from "solid-js";

interface InputToggleProps {
	label: string;
	desc?: string;
	value?: boolean;
	onChange?: (event: Event) => void;
	onToggle?: (value: boolean) => void;
}

const instanceId = createUniqueId();

export const InputToggle: Component<InputToggleProps> = (props) => {
	const merged = mergeProps(
		{
			desc: null,
			value: false,
			onChange: () => {},
			onToggle: () => {},
		},
		props,
	);

	const name = $memo(merged.label.replace(/\s+/g, ""));

	const handle = (e: Event) => {
		merged.onChange(e);
		merged.onToggle((e.target as HTMLInputElement).checked);
	};

	return (
		<div class="o-form-group keep-size">
			<legend>{merged.label.replace(/([A-Z])/g, " $1")}</legend>
			<Show when={merged.desc}>
				<p class="desc">{merged.desc}</p>
			</Show>

			<div class="toggle-input">
				<input type="checkbox" name={name} id={instanceId + name} checked={merged.value} onchange={handle} />
				<label for={instanceId + name}>
					<span class="toggle">
						<span class="dot" />
					</span>
					{merged.value ? "On" : "Off"}
				</label>
			</div>
		</div>
	);
};
