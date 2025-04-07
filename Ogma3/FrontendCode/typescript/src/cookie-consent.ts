import { setCookie } from "@h/cookies";
import { addToDate } from "@h/date-helpers";

const banner = document.getElementById("cookie-consent");
const button = banner.querySelector("button#cookie-consent-button");

button.addEventListener("click", () => {
	const expiry = addToDate(new Date(), { years: 100 });

	setCookie("cookie-consent", "true", { expires: expiry, secure: true, sameSite: "Strict", path: "/" });
	banner.style.display = "none";
});
