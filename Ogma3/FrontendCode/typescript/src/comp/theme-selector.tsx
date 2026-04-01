import { addToDate } from "@angius/tinytime/addtoDate";
import { useClickOutside } from "@h/click-outside";
import { $query } from "@h/dom";
import { getThemes } from "@h/macros/get-theme-names.macro" with { type: "macro" };
import { component } from "@h/web-components";
import { type CookieOptions, cookieStorage, makePersisted } from "@solid-primitives/storage";
import { capitalize } from "es-toolkit";
import type { ComponentType } from "solid-element";
import { createEffect, createSignal, For } from "solid-js";
import css from "./theme-selector.css";

const themeMap = await getThemes();
const themes = Object.keys(themeMap);

const preferredTheme = window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";

const themeLink = $query("link#theme-ph");

const ThemeSelector: ComponentType<null> = (_props, { element }) => {
	const details = $signal<HTMLDetailsElement>();

	const [theme, setTheme] = makePersisted(createSignal<string>("system"), {
		storage: cookieStorage,
		name: "theme",
		storageOptions: {
			sameSite: "Strict",
			expires: addToDate(new Date(), { years: 1 }),
			httpOnly: false,
		} satisfies CookieOptions,
	});

	createEffect(() => {
		const t = theme();
		const name = t === "system" ? preferredTheme : t;
		themeLink.setAttribute("rel", "stylesheet");
		themeLink.setAttribute("href", `/css/${name}.css?v=${themeMap[name]}`);
	});

	useClickOutside(element, () => {
		details?.removeAttribute("open");
	});

	let stylesLoaded = $signal(false);
	const onToggle = (e: ToggleEvent) => {
		if (e.newState === "open" && !stylesLoaded) {
			for (const theme of themes) {
				const preloader = document.createElement("link");
				preloader.rel = "preload";
				preloader.as = "style";
				preloader.href = `/css/${theme}.css?v=${themeMap[theme]}`;
				document.head.appendChild(preloader);
			}
			stylesLoaded = true;
		}
		if (e.newState === "closed") {
			return;
		}
		const focusTarget = details?.querySelector("input:checked") || details?.querySelector("input");
		(focusTarget as HTMLElement)?.focus();
	};

	let isPointerDown = $signal(false);

	const onPointerDown = () => {
		isPointerDown = true;
		window.addEventListener(
			"pointerup",
			() => {
				isPointerDown = false;
			},
			{ once: true },
		);
	};

	const onFocusOut = (e: FocusEvent) => {
		if (isPointerDown) {
			return;
		}

		const current = e.currentTarget as HTMLElement;

		if (e.relatedTarget && current.contains(e.relatedTarget as HTMLElement)) {
			return;
		}

		queueMicrotask(() => {
			const root = current.getRootNode() as ShadowRoot | Document;
			if (current.contains(root.activeElement)) {
				return;
			}
			details?.removeAttribute("open");
		});
	};

	return (
		<details
			class="dropdown"
			ref={$set(details)}
			ontoggle={onToggle}
			onfocusout={onFocusOut}
			onpointerdown={onPointerDown}
		>
			<summary aria-haspopup="true" aria-controls="theme-list" aria-label="Select theme">
				{capitalize(theme())} theme
			</summary>

			<div id="theme-list" class="options" role="listbox" aria-label="Themes">
				<For each={["system", ...themes]}>
					{(t) => (
						<label>
							<input
								type="radio"
								name="theme-selector"
								value={t}
								checked={theme() === t}
								onchange={[setTheme, t]}
							/>
							{capitalize(t)} theme
						</label>
					)}
				</For>
			</div>
		</details>
	);
};

component("theme-selector", {}, ThemeSelector, css);
