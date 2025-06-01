import { getCookieValue, setCookie } from "@h/cookies";
import { $id, $query } from "@h/dom";
import { useLocalStorage } from "@h/localStorageHook";
import { createEffect, createSignal } from "solid-js";
import { render } from "solid-js/web";

const LocalSettings = () => {
	const [getCollapseDeleted, setCollapseDeleted] = useLocalStorage<boolean>("collapse-deleted", false);
	const [getTheme, setTheme] = createSignal(((x) => ((x?.length ?? 0) > 0 ? x : "light"))(getCookieValue("theme")));

	createEffect(() => {
		console.log(getCollapseDeleted());
	});

	const swapTheme = () => {
		const themeLink = $query("link#theme-ph");
		const rnd = Math.random()
			.toString(36)
			.replace(/[^a-z]+/g, "")
			.slice(0, 5);
		const date = new Date();
		date.setFullYear(date.getFullYear() + 100);
		const theme = getTheme() === "dark" ? "light" : "dark";
		setTheme(theme);
		themeLink.setAttribute("rel", "stylesheet");
		themeLink.setAttribute("href", `/css/${theme}.css?v=${rnd}`);
		setCookie("theme", theme, { expires: date, secure: false, sameSite: "Lax" });
	};

	const toggleCollapseDeleted = () => {
		setCollapseDeleted(!getCollapseDeleted());
	};

	return (
		<>
			<dt>Collapse deleted comments</dt>
			<dd>
				<button type="button" onClick={toggleCollapseDeleted} classList={{ on: getCollapseDeleted() ?? false }}>
					{getCollapseDeleted() ? "on" : "off"}
				</button>
			</dd>

			<dt>Theme</dt>
			<dd>
				<button type="button" onClick={swapTheme}>
					{getTheme()}
				</button>
			</dd>
		</>
	);
};

render(() => <LocalSettings />, $id("local-settings"));
