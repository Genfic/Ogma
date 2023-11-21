import { setCookie } from "../src-helpers/cookies";

const banner = document.getElementById("cookie-consent");
const button = banner.querySelector("button#cookie-consent-button");

button.addEventListener("click", (_) => {
	const expiry = new Date();
	expiry.setDate(expiry.getDate() + 400);
		
	setCookie('cookie-consent', 'true', expiry, true, 'strict', '/');
	banner.style.display = 'none';
});