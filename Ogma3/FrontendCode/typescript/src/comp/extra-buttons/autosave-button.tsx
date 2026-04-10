import LucideDownload from "icon:lucide:download";
import LucideSave from "icon:lucide:save";
import LucideSaveOff from "icon:lucide:save-off";
import { component } from "@h/web-components";
import { debounce } from "es-toolkit";
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

		input.addEventListener("keydown", onKeyDown);
		onCleanup(() => {
			input.removeEventListener("keydown", onKeyDown);
		});
	});

	const onKeyDown = (e: KeyboardEvent) => {
		if (!enabled) return;
		if (![" ", "backspace", "enter"].includes(e.key.toLowerCase())) return;

		debounce(() => {
			const input = props.context.input;

			if (input.value.length > 0) {
				localStorage.setItem(key, input.value);
			} else {
				localStorage.removeItem(key);
			}
		}, 200)();
	};

	let enabled = $signal(true);

	const toggle = () => {
		enabled = !enabled;
	};

	const load = () => {
		props.context.input.value = localStorage.getItem(key) ?? "";
	};

	return (
		<>
			<button
				type="button"
				class="btn action-btn"
				title={`Autosave is ${enabled ? "enabled" : "disabled"}`}
				onClick={toggle}
			>
				{enabled ? <LucideSave /> : <LucideSaveOff />}
			</button>
			<Show when={saveExists}>
				<button type="button" class="btn action-btn" title="Load existing autosave" onClick={load}>
					<LucideDownload />
				</button>
			</Show>
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
