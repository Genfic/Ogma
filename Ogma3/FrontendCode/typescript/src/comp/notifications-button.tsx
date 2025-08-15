import { GetApiNotificationsCount as countNotifications } from "@g/paths-public";
import type { Empty } from "@t/utils";
import { type ComponentType, customElement } from "solid-element";
import { createResource, Show } from "solid-js";
import { LucideBell } from "../icons/LucideBell";
import { Styled } from "./common/_styled";
import css from "./notifications-button.css";

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
			<LucideBell />
			<Show when={notifications() > 0}>
				<span>{count()}</span>
			</Show>
		</a>
	);
};

customElement("o-notifications-button", {}, Styled(NotificationsButton, css));
