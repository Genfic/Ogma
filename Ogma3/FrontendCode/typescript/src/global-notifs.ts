import { Cookies } from "@g/ctconfig";
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
		const cookie = await cookieStore.get(COOKIE_NAME);
		console.log("cookie:", cookie);

		const dismissed = cookie?.value?.split(",") ?? [];
		dismissed.push(id);

		console.log(dismissed);

		await cookieStore.set(COOKIE_NAME, dismissed.join(","));
		notif.remove();
	});
}
