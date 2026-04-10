import LucideDownload from "icon:lucide:download";
import LucideSave from "icon:lucide:save";
import LucideSaveOff from "icon:lucide:save-off";
import { component } from "@h/web-components";
import { debounce } from "@solid-primitives/scheduled";
import type { ComponentType } from "solid-element";
import { createEffect, onCleanup, Show } from "solid-js";
import shared from "../shared.css";
import css from "./autosave-button.css";
import type { ExtraButtonContext } from "./extra-button-types";

const AutosaveButton: ComponentType<{ context: ExtraButtonContext; key: string; uid: number }> = (props) => {
	const key = `autosave:${props.key}:${props.uid}`;

	let saveExists = $signal(false);

	createEffect(() => {
		const input = props.context.input;

		if (!(input instanceof EventTarget)) return;

		saveExists = (localStorage.getItem(key)?.length ?? -1) > 0;

		input.addEventListener("input", save);
		onCleanup(() => {
			input.removeEventListener("input", save);
		});
	});

	const save = debounce(() => {
		if (!enabled) return;

		const input = props.context.input;

		if (input.value.length > 0) {
			console.log("Autosave");
			localStorage.setItem(key, input.value);
		} else {
			localStorage.removeItem(key);
		}
	}, 1000);

	let enabled = $signal(true);

	const load = () => {
		props.context.input.value = localStorage.getItem(key) ?? "";
	};

	return (
		<>
			<Show when={saveExists}>
				<button type="button" class="btn action-btn" title="Load existing autosave" onClick={load}>
					<LucideDownload />
				</button>
			</Show>
			<button
				type="button"
				class="btn action-btn"
				title={`Autosave is ${enabled ? "enabled" : "disabled"}`}
				onClick={() => (enabled = !enabled)}
			>
				{enabled ? <LucideSave /> : <LucideSaveOff />}
			</button>
		</>
	);
};

component(
	"autosave-btn",
	{ context: { input: {} as HTMLInputElement }, key: "", uid: -1 },
	AutosaveButton,
	[shared, css],
	["context"],
);
