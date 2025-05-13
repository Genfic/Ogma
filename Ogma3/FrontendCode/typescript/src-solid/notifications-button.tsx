import { GetApiNotificationsCount as countNotifications } from "@g/paths-public";
import { type ComponentType, customElement } from "solid-element";
import { Show, createResource } from "solid-js";
import css from "./notifications-button.css";
import { Styled } from "./common/_styled";
import type { Empty } from "@t/utils";

const NotificationsButton: ComponentType<Empty> = (_) => {
	const [notifications] = createResource(
		async () => {
			const res = await countNotifications();
			return res.ok ? res.data : -1;
		},
		{ initialValue: 0 },
	);

	const count = () => (notifications() <= 99 ? notifications().toString() : "99+");

	const linkTitle = () => (notifications() > 0 ? `${notifications()} notifications` : "Notifications");

	return (
		<a class="nav-link light notifications-btn" href="/notifications" title={linkTitle()}>
			<o-icon class="material-icons-outlined" icon="lucide:bell" />
			<Show when={notifications() > 0}>
				<span>{count()}</span>
			</Show>
		</a>
	);
};

customElement("o-notifications-button", {}, Styled(NotificationsButton, css));
