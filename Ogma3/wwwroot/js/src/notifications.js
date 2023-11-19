import dayjs from "dayjs";

new Vue({
	el: "#notifications",
	data: {
		notifications: [],
		route: null,
		csrf: null,
	},
	methods: {
		fetch: async function () {
			const { data } = await axios.get(this.route);
			this.notifications = data;
		},

		deleteNotif: async function (id) {
			await axios.delete(`${this.route}/${id}`, {
				headers: { RequestVerificationToken: this.csrf },
			});
			await this.fetch();
		},

		parseTime: function (time) {
			return dayjs(time).format("DD MMMM YYYY, HH:mm");
		},
	},
	async mounted() {
		this.route = document.querySelector("[data-route]").dataset.route;
		await this.fetch();
		this.csrf = document.querySelector("input[name=__RequestVerificationToken]").value;
	},
}); 