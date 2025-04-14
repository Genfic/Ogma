import { GetApiNotificationsCount as countNotifications } from "@g/paths-public";
import { log } from "@h/logger";
import { type ComponentType, customElement, noShadowDOM } from "solid-element";
import { Show, createSignal, onMount } from "solid-js";

const NotificationsButton: ComponentType<null> = (_, { element }) => {
	noShadowDOM();

	const [notifications, setNotifications] = createSignal(null);

	onMount(async () => {
		element.classList.add("wc-loaded");

		const res = await countNotifications();
		if (res.ok) {
			setNotifications(res.data);
		} else {
			log.error(res.statusText);
		}
	});

	const count = () => (notifications() <= 99 ? notifications().toString() : "99+");

	const linkTitle = () => (notifications() > 0 ? `${notifications()} notifications` : "Notifications");

	return (
		<a class="nav-link light notifications-btn" href="/notifications" title={linkTitle()}>
			<o-icon class="material-icons-outlined" icon="lucide:bell" />
			<Show when={(notifications() ?? -1) > 0}>
				<span>{count()}</span>
			</Show>
		</a>
	);
};

customElement("o-notifications-button", null, NotificationsButton);
