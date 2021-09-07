import { NotificationsClient } from "../../lib/openapi/OpenAPI";
import './site';

(async () => {
	const notificationBtn = document.querySelector("[data-notifications]") as HTMLButtonElement;
	if (!notificationBtn) return;

	let na = new NotificationsClient();
	let count = await na.countUserNotifications();

	if ((count ?? 0) > 0) {
		notificationBtn.title = `${count} notifications`;
		const child = document.createElement("span");
		child.innerText = count <= 99
			? count.clamp(0, 99).toString()
			: '99+';
		notificationBtn.appendChild(child);
	}
})();