import { DeleteApiNotifications as deleteNotification, GetApiNotifications as getNotifications } from "@g/paths-public";
import { $id } from "@h/dom";
import { long } from "@h/tinytime-templates";
import { createResource, createSignal, For, Match, Switch } from "solid-js";
import { render } from "solid-js/web";

const parent = $id("notifications");

const Notifications = () => {
	const [notifications, { refetch }] = createResource(async () => {
		const res = await getNotifications();
		if (!res.ok) {
			throw new Error(res.error);
		}
		return res.data;
	});
	const [csrf] = createSignal(parent.dataset.csrf ?? "");

	const deleteNotif = async (id: number) => {
		const res = await deleteNotification(id, {
			RequestVerificationToken: csrf(),
		});
		if (!res.ok) return;
		refetch();
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
				<For each={notifications()}>
					{(notif) => (
						<div class="notification active-border">
							<a class="link" href={notif.url}>
								{notif.message}
							</a>
							<span class="body">{notif.body}</span>
							<span class="time">{long.render(new Date(notif.dateTime))}</span>
							<div class="actions">
								<button type="button" onClick={[deleteNotif, notif.id]}>
									<o-icon icon="lucide:trash-2" />
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
