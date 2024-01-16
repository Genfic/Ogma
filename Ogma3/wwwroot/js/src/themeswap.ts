import { getCookieValue, setCookie } from "../src-helpers/cookies";

const themeLink = document.querySelector("link#theme-ph") as HTMLLinkElement;
const themeBtn = document.getElementById("theme-swap") as HTMLButtonElement;

const rnd: string = Math.random()
	.toString(36)
	.replace(/[^a-z]+/g, "")
	.slice(0, 5);

const date = new Date();

date.setFullYear(date.getFullYear() + 100);

themeBtn.addEventListener("click", () => {
	const theme = getCookieValue("theme") === "dark" ? "light" : "dark";

	themeLink.setAttribute("rel", "stylesheet");
	themeLink.setAttribute("href", `/css/${theme}.min.css?v=${rnd}`);

	setCookie("theme", theme, date, true, "lax");
});
