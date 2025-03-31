import { getCookieValue, setCookie } from "../src-helpers/cookies";
import { addToDate } from "../src-helpers/date-helpers";
import { timedString } from "../src-helpers/generators";

const themeLink = document.querySelector("link#theme-ph") as HTMLLinkElement;
const themeBtn = document.getElementById("theme-swap") as HTMLButtonElement;

themeBtn.addEventListener("click", () => {
	const theme = getCookieValue("theme") === "dark" ? "light" : "dark";

	themeLink.setAttribute("rel", "stylesheet");
	themeLink.setAttribute("href", `/css/${theme}.min.css?v=${timedString()}`);

	const date = addToDate(new Date(), { years: 100 });

	setCookie("theme", theme, { expires: date, secure: true, sameSite: "Lax" });
});
