new Vue({
	el: "#profile-bar",
	data: {
		route: null,
		name: null,

		isBlocked: null,
		isFollowed: null,

		xcsrf: null,

		done: false,
	},
	methods: {
		follow: async function () {
			const res = await fetch(`${this.route}/follow`, {
				method: this.isFollowed ? "DELETE" : "POST",
				headers: {
					RequestVerificationToken: this.xcsrf,
					"Content-Type": "application/json",
				},
				body: JSON.stringify({ name: this.name }),
			});
			this.isFollowed = (await res.text()).toLowerCase() === "true";
		},

		block: async function () {
			const res = await fetch(`${this.route}/block`, {
				method: this.isBlocked ? "DELETE" : "POST",
				headers: {
					RequestVerificationToken: this.xcsrf,
					"Content-Type": "application/json",
				},
				body: JSON.stringify({ name: this.name }),
			});
			this.isBlocked = (await res.text()).toLowerCase() === "true";
		},

		report: function () {
			this.$refs.reportModal.visible = true;
		},
	},
	mounted() {
		this.route = document.getElementById("data-route").dataset.route;
		this.name = document.getElementById("data-name").dataset.name;

		this.isBlocked =
			document
				.getElementById("data-blocked")
				.dataset.blocked.toLowerCase() === "true";
		this.isFollowed =
			document
				.getElementById("data-followed")
				.dataset.followed.toLowerCase() === "true";

		this.xcsrf = document.querySelector(
			"[name=__RequestVerificationToken]"
		).value;
		this.done = true;
	},
});