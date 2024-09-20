import { getCookieValue, setCookie } from "../src-helpers/cookies";
import { timedString } from "../src-helpers/generators";
import { addYears } from "date-fns";

const themeLink = document.querySelector("link#theme-ph") as HTMLLinkElement;
const themeBtn = document.getElementById("theme-swap") as HTMLButtonElement;

themeBtn.addEventListener("click", () => {
	const theme = getCookieValue("theme") === "dark" ? "light" : "dark";

	themeLink.setAttribute("rel", "stylesheet");
	themeLink.setAttribute("href", `/css/${theme}.min.css?v=${timedString()}`);

	const date = addYears(new Date(), 100);

	setCookie("theme", theme, date, true, "lax");
});
