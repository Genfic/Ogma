import { $query } from "@h/dom";
import { component } from "@h/web-components";
import { makePersisted } from "@solid-primitives/storage";
import { type ComponentType, noShadowDOM } from "solid-element";
import { createEffect, createUniqueId } from "solid-js";
import { createStore } from "solid-js/store";
import { LucideSettings } from "../icons/LucideSettings";
import { Dialog, type DialogApi } from "./common/_dialog";
import { InputToggle } from "./common/_input-toggle";
import css from "./reader-settings.css";

const unitMap = {
	"--chapter-align": "",
	"--chapter-para-spacing": "rem",
	"--chapter-line-height": "",
	"--chapter-font-size": "rem",
} as const;

const cssVars = Object.keys(unitMap) as readonly (keyof typeof unitMap)[];

type CSSVar = (typeof cssVars)[number];

const ReaderSettings: ComponentType<never> = () => {
	noShadowDOM();

	const uid = createUniqueId();
	const $id = (id: string) => `${uid}-${id}`;

	const articleBody = $query("article.chapter-details div.body[itemprop='text']");

	const defaults = Object.freeze(
		cssVars.reduce(
			(acc, curr) => {
				const value = getComputedStyle(articleBody).getPropertyValue(curr);
				acc[curr] = value.replace(unitMap[curr], "");
				return acc;
			},
			{} as Record<CSSVar, string>,
		),
	);

	const dialog = $signal<DialogApi>();
	const [getStore, setStore] = makePersisted(createStore({ ...defaults }), { name: "reader-settings" });

	createEffect(() => {
		for (const [name, value] of Object.entries(getStore)) {
			articleBody.style.setProperty(name, value + unitMap[name as CSSVar]);
		}
	});

	const setVar = (name: (typeof cssVars)[number]) => {
		return (e: Event) => {
			if (!e.target) {
				return;
			}
			setStore(name, (e.target as HTMLInputElement).value);
		};
	};

	const reset = () => {
		console.log("Resetting", getStore, defaults);
		for (const key of cssVars) {
			setStore(key, defaults[key]);
		}
		console.log("Reset", getStore, defaults);
	};

	return (
		<>
			<button type="button" class="settings-button" title="Change reader settings" onclick={() => dialog?.open()}>
				<LucideSettings />
			</button>
			<Dialog ref={$set(dialog)}>
				<InputToggle
					label="justify"
					value={getStore["--chapter-align"] === "justify"}
					onToggle={(b) => setStore("--chapter-align", b ? "justify" : "left")}
				/>
				<label for={$id("spacing")}>Paragraph spacing: {getStore["--chapter-para-spacing"]}</label>
				<input
					id={$id("spacing")}
					type="range"
					min="0"
					max="5"
					step="0.05"
					value={getStore["--chapter-para-spacing"]}
					oninput={setVar("--chapter-para-spacing")}
				/>
				<label for={$id("line-height")}>Line height: {getStore["--chapter-line-height"]}</label>
				<input
					id={$id("line-height")}
					type="range"
					min="0.75"
					max="5"
					step="0.01"
					value={getStore["--chapter-line-height"]}
					oninput={setVar("--chapter-line-height")}
				/>
				<label for={$id("font-size")}>Font size: {getStore["--chapter-font-size"]}</label>
				<input
					id={$id("font-size")}
					type="range"
					min="0.5"
					max="5"
					step="0.05"
					value={getStore["--chapter-font-size"]}
					oninput={setVar("--chapter-font-size")}
				/>

				<br />

				<button class="btn" type="button" onclick={reset}>
					Reset
				</button>
			</Dialog>
		</>
	);
};

component("reader-settings", {}, ReaderSettings, css);
