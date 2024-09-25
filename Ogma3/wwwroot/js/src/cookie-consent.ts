import { addYears } from "date-fns";
import { setCookie } from "../src-helpers/cookies";

const banner = document.getElementById("cookie-consent");
const button = banner.querySelector("button#cookie-consent-button");

button.addEventListener("click", () => {
	const expiry = addYears(new Date(), 100);

	setCookie("cookie-consent", "true", expiry, true, "strict", "/");
	banner.style.display = "none";
});
