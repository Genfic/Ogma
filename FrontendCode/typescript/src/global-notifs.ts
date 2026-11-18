import { Cookies } from "@g/ctconfig";
import { getCookieValue, setCookie } from "@h/cookies";
import { $queryAll } from "@h/dom";

const COOKIE_NAME = Cookies.DismissedNotifications;

const notifs = $queryAll("[global-notif-id]");

console.log("Found notifications:", notifs);

for (const notif of notifs) {
	const id = notif.getAttribute("global-notif-id") ?? "0";

	const btn = notif.getElementsByTagName("button")[0];

	console.log(`Notification ${id} has button:`, btn);

	btn.addEventListener("click", async () => {
		console.log("Clicked");
		const cookie = await getCookieValue(COOKIE_NAME);
		console.log("cookie:", cookie);

		const dismissed = new Set(cookie?.split(","));
		dismissed.add(id);

		console.log(dismissed);

		await setCookie(COOKIE_NAME, [...dismissed].join(","), {
			sameSite: "Lax",
			expires: Date.now() + 60 * 60 * 60 * 24 * 365 * 10,
			path: "/",
		});
		notif.remove();
	});
}
