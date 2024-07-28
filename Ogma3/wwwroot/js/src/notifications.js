import { DeleteApiNotifications as deleteNotification, GetApiNotifications as getNotifications } from "../generated/paths-public";
import dayjs from "dayjs";

new Vue({
	el: "#notifications",
	data: {
		notifications: [],
		csrf: null,
	},
	methods: {
		load: async function () {
			const data = await getNotifications();
			this.notifications = await data.json();
		},

		deleteNotif: async function (id) {
			await deleteNotification(id, {
				RequestVerificationToken: this.csrf,
			});
			await this.load();
		},

		parseTime: (time) => dayjs(time).format("DD MMMM YYYY, HH:mm"),
	},

	async mounted() {
		await this.load();
		this.csrf = document.querySelector("input[name=__RequestVerificationToken]").value;
	},
});
