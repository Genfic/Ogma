new Vue({
	el: "#app",
	data: {
		codes: [],
		route: null,
		xcsrf: null
	},
	methods: {

		createCode: async function() {
			const { data } = await axios.post(this.route, null,
				{ headers: { RequestVerificationToken: this.xcsrf } }
			).catch(e => alert(e.response.data));
			this.codes.push(data);
		},

		// Gets all existing namespaces
		getCodes: async function() {
			const { data } = await axios.get(this.route);
			this.codes = data;
		},

		copyCode: function(t) {
			navigator.clipboard.writeText(t.code).then(
				() => alert("Copied"),
				(e) => {
					alert("Could not copy");
					console.error(e);
				}
			);
		},

		// Parse date
		date: function(dt) {
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