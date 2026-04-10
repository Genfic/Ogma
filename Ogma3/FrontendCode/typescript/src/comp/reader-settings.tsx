import LucideSettings from "icon:lucide:settings";
import { CSS } from "@h/css-helpers";
import { $query } from "@h/dom";
import { component } from "@h/web-components";
import { cookieStorage, makePersisted } from "@solid-primitives/storage";
import { type ComponentType, noShadowDOM } from "solid-element";
import { createEffect, createUniqueId } from "solid-js";
import { createStore } from "solid-js/store";
import { Dialog, type DialogApi } from "./common/_dialog";
import { InputToggle } from "./common/_input-toggle";
import css from "./reader-settings.css";

const cssVars = ["--align", "--para-spacing", "--line-height", "--font-size", "--bg-color", "--fg-color"] as const;
type CSSVar = (typeof cssVars)[number];

const articleBody = $query("article.chapter-details div.body[itemprop='text']");

// We have to remove the contents rendered from the server so that the computed style
// actually uses the defaults from the stylesheet instead of the customized ones.
const savedStyle = articleBody.style.cssText;
articleBody.style.cssText = "";

const defaults = Object.freeze(
	cssVars.reduce(
		(acc, curr) => {
			acc[curr] = getComputedStyle(articleBody).getPropertyValue(curr);
			return acc;
		},
		{} as Record<CSSVar, string>,
	),
);

// and it needs to be restored
articleBody.style.cssText = savedStyle;

const ReaderSettings: ComponentType<never> = () => {
	noShadowDOM();

	const uid = createUniqueId();
	const $id = (id: string) => `${uid}-${id}`;

	const dialog = $signal<DialogApi>();
	const [getStore, setStore] = makePersisted(createStore({ ...defaults }), {
		name: "reader-settings",
		storage: cookieStorage,
		storageOptions: {
			path: "/",
			maxAge: 365 * 24 * 60 * 60,
			sameSite: "strict",
		},
		serialize: CSS.serialize,
		deserialize: CSS.deserialize<CSSVar>,
	});

	createEffect(() => {
		for (const [name, value] of Object.entries(getStore)) {
			articleBody.style.setProperty(name, value);
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
					value={getStore["--align"] === "justify"}
					onToggle={(b) => setStore("--align", b ? "justify" : "left")}
				/>
				<label for={$id("spacing")}>Paragraph spacing: {getStore["--para-spacing"]}</label>
				<input
					id={$id("spacing")}
					type="range"
					min="0"
					max="5"
					step="0.05"
					value={getStore["--para-spacing"]}
					oninput={setVar("--para-spacing")}
				/>
				<label for={$id("line-height")}>Line height: {getStore["--line-height"]}</label>
				<input
					id={$id("line-height")}
					type="range"
					min="0.75"
					max="5"
					step="0.01"
					value={getStore["--line-height"]}
					oninput={setVar("--line-height")}
				/>
				<label for={$id("font-size")}>Font size: {getStore["--font-size"]}</label>
				<input
					id={$id("font-size")}
					type="range"
					min="0.5"
					max="5"
					step="0.05"
					value={getStore["--font-size"]}
					oninput={setVar("--font-size")}
				/>
				<label for={$id("bg-color")}>Background color</label>
				<input
					id={$id("bg-color")}
					type="color"
					value={CSS.toHex(getStore["--bg-color"])}
					oninput={setVar("--bg-color")}
				/>
				<label for={$id("fg-color")}>Text color</label>
				<input
					id={$id("fg-color")}
					type="color"
					value={CSS.toHex(getStore["--fg-color"])}
					oninput={setVar("--fg-color")}
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
