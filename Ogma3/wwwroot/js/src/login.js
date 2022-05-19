new Vue({
	el: "#app",
	data: {
		name: null,
		avatar: "https://picsum.photos/200",
		title: null,
		checked: false,

		route: null,
	},
	methods: {
		checkDetails: async function (e) {
			e.preventDefault();

			if (this.name) {
				const { data, status } = await axios.get(
					`${this.route}?name=${this.name}`
				);

				if (status === 200) {
					this.avatar = data.avatar;
					this.title = data.title;
					this.checked = true;
				}
			}
		},
		reset: function () {
			this.avatar = null;
			this.title = null;
			this.checked = false;
		},
	},

	mounted() {
		// Grab the route from route helper
		this.route = document.getElementById("route").dataset.route;
	},
});

