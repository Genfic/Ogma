import { addToDate } from "@angius/tinytime/addtoDate";
import { setCookie } from "@h/cookies";
import { $id, $query } from "@h/dom";

const banner = $id("cookie-consent");
const button = $query<HTMLButtonElement>("button#cookie-consent-button");

button.addEventListener("click", () => {
	const expiry = addToDate(new Date(), { years: 100 });

	setCookie("cookie-consent", "true", { expires: expiry, secure: true, sameSite: "Strict", path: "/" });
	banner.style.display = "none";
});
