import {
	DeleteApiNotifications as deleteNotification,
	GetApiNotifications as getNotifications,
} from "../generated/paths-public";
import type { GetUserNotificationsResult } from "../generated/types-public";
import { long } from "../src-helpers/tinytime-templates";

// @ts-ignore
new Vue({
	el: "#notifications",
	data: {
		notifications: [] as GetUserNotificationsResult[],
		csrf: null,
	},
	methods: {
		load: async function () {
			const data = await getNotifications();

			if (!data.ok) return;

			this.notifications = data.data;
		},

		deleteNotif: async function (id: number) {
			const res = await deleteNotification(id, {
				RequestVerificationToken: this.csrf,
			});

			if (!res.ok) return;

			await this.load();
		},

		parseTime: (dt: string) => long.render(new Date(dt)),
	},

	async mounted() {
		await this.load();
		this.csrf = (document.querySelector("input[name=__RequestVerificationToken]") as HTMLInputElement).value;
	},
});
