import {
	DeleteApiNotificationsAll as deleteAllNotifications,
	DeleteApiNotifications as deleteNotification,
	GetApiNotifications as getNotifications,
} from "@g/paths-public";
import { toCurrentTimezone } from "@h/date-helpers";
import { $id } from "@h/dom";
import { long } from "@h/tinytime-templates";
import LucideTrash2 from "icon:lucide:trash-2";
import { createResource, For, Match, Switch } from "solid-js";
import { render } from "solid-js/web";

const parent = $id("notifications");

const Notifications = () => {
	const [notifications, { refetch, mutate }] = createResource(async () => {
		const res = await getNotifications();
		if (!res.ok) {
			throw new Error(res.statusText);
		}
		return res.data;
	});
	const csrf = parent.dataset.csrf ?? "";

	const deleteNotif = async (id: number) => {
		const res = await deleteNotification(id, {
			RequestVerificationToken: csrf,
		});
		if (!res.ok) return;
		refetch();
	};

	const deleteAll = async () => {
		const res = await deleteAllNotifications({
			RequestVerificationToken: csrf,
		});
		if (!res.ok) return;
		mutate([]);
	};

	return (
		<Switch>
			<Match when={notifications.loading}>
				<span class="loading">Loading...</span>
			</Match>
			<Match when={notifications.error}>
				<span class="error">{notifications.error}</span>
			</Match>
			<Match when={notifications()}>
				{(notifications()?.length ?? 0) <= 0 ? (
					<h2>You're all set! The inbox is empty.</h2>
				) : (
					<button type="button" class="btn btn-primary" onclick={deleteAll}>
						<LucideTrash2 />
						Clear all
					</button>
				)}
				<For each={notifications()}>
					{(notif) => (
						<div class="notification active-border">
							<a class="link" href={notif.url}>
								{notif.message}
							</a>
							<span class="body">{notif.body}</span>
							<span class="time">{long.render(toCurrentTimezone(new Date(notif.dateTime)))}</span>
							<div class="actions">
								<button type="button" class="action-btn" onClick={[deleteNotif, notif.id]}>
									<LucideTrash2 />
								</button>
							</div>
						</div>
					)}
				</For>
			</Match>
		</Switch>
	);
};

render(() => <Notifications />, parent);
