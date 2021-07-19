new Vue({
	el: "#app",
	data: {
		form: {
			body: null,
			author: null
		},
		quotes: [],
		route: null,
		json: null,
		search: ""
	},
	methods: {

		// Gets all existing namespaces
		getQuotes: async function() {
			const { data } = await axios.get(this.route);
			this.quotes = data;
		},

		deleteQuote: async function(q) {
			if (confirm("Delete permanently?")) {
				const { data } = await axios.delete(this.route, { data: { id: q.id }});
				this.quotes = this.quotes.filter(i => i.id !== data.id);
			}
		},

		editQuote: function() {
		},

		// Upload Json
		fromJson: async function() {
			const res = await axios.post("/api/quotes/json", JSON.parse(this.json));
			console.log(res);
		}
	},

	watch: {
		search() {
			for (const q of this.quotes) {
				q.show = this.search
					? q.body.toLowerCase().includes(this.search.toLowerCase()) || q.author.toLowerCase().includes(this.search.toLowerCase())
					: true;
			}
		}
	},

	async mounted() {
		// Grab the route from route helper
		this.route = document.getElementById("route").dataset.route;
		// Grab the initial set of namespaces
		await this.getQuotes();
	}
});