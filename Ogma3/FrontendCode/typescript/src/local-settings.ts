import { getCookieValue, setCookie } from "@h/cookies";

// @ts-ignore
const settings_vue = new Vue({
	el: "#local-settings",
	data: {
		// Comment collapse
		collapseDeleted: JSON.parse(window.localStorage.getItem("collapse-deleted")) ?? false,

		// Theme
		theme: ((x) => (x?.length > 0 ? x : "light"))(getCookieValue("theme")),
	},
	methods: {
		updateCollapse: function () {
			this.collapseDeleted = !this.collapseDeleted;
			window.localStorage.setItem("collapse-deleted", this.collapseDeleted);
		},

		swapTheme: function () {
			const themeLink = document.querySelector("link#theme-ph");

			const rnd = Math.random()
				.toString(36)
				.replace(/[^a-z]+/g, "")
				.slice(0, 5);
			const date = new Date();
			date.setFullYear(date.getFullYear() + 100);

			const theme = this.theme === "dark" ? "light" : "dark";
			this.theme = theme;

			themeLink.setAttribute("rel", "stylesheet");
			themeLink.setAttribute("href", `/css/${theme}.css?v=${rnd}`);

			setCookie("theme", theme, { expires: date, secure: false, sameSite: "Lax" });
		},
	},
});
