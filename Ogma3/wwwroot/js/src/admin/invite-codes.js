new Vue({
	el: "#app",
	data: {
		codes: [],
		route: null,
		xcsrf: null,
		page: 1,
		perPage: 5,
		loading: true,
		completed: false,
		newCode: null
	},
	methods: {

		createCode: async function () {
			const { data } = await axios.post(`${this.route}/no-limit`, null, {
				headers: { RequestVerificationToken: this.xcsrf }
			});

			this.newCode = data.id;
			setTimeout(() => this.newCode = null, 5000);

			this.codes.unshift(data);
		},

		// Gets page of codes
		getCodes: async function () {
			if (this.completed) return;

			this.loading = true;
			const { data } = await axios.get(`${this.route}/paginated`, {
				params: {
					page: this.page,
					perPage: this.perPage
				}
			});

			if (data.length <= 0) {
				this.completed = true;
				return;
			}

			this.codes = [...this.codes, ...data];
			this.loading = false;
		},

		loadMore: async function () {
			this.page++;
			await this.getCodes();
		},

		// Deletes a selected namespace
		deleteCode: async function (t) {
			if (confirm("Delete permanently?")) {
				await axios.delete(`${this.route}/${t.id}`, {
					headers: { RequestVerificationToken: this.xcsrf }
				});
				this.codes = this.codes.filter(i => i.id !== t.id);
			}
		},

		copyCode: function (t) {
			navigator.clipboard.writeText(t.code).then(
				() => alert("Copied"),
				(e) => {
					alert("Could not copy");
					log.error(e);
				}
			);
		},

		// Parse date
		date: function (dt) {
			return dayjs(dt).format("DD MMM YYYY, HH:mm");
		}
	},

	async mounted() {
		// Grab the route from route helper
		this.route = document.getElementById("route").dataset.route;
		// Grab the XCSRF token
		this.xcsrf = document.querySelector("[name=__RequestVerificationToken]").value;
		// Grab the initial set of namespaces
		await this.getCodes();
	}
});