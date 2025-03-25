import { setCookie } from "../src-helpers/cookies";
import { addToDate } from "../src-helpers/date-helpers";

const banner = document.getElementById("cookie-consent");
const button = banner.querySelector("button#cookie-consent-button");

button.addEventListener("click", () => {
	const expiry = addToDate(new Date(), { years: 100 });

	setCookie("cookie-consent", "true", expiry, true, "strict", "/");
	banner.style.display = "none";
});
