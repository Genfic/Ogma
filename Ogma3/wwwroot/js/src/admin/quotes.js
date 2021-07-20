new Vue({
	el: "#app",
	data: {
		form: {
			id: null,
			body: null,
			author: null
		},
		quotes: [],
		route: null,
		json: null,
		search: "",
		
		editorOpen: false,
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
		
		openEditor: function(q) {
			this.editorOpen = true;
			this.form = q;
		},
		closeEditor: function() {
			this.editorOpen = false;
			Object.keys(this.form).forEach((i) => this.form[i] = null);
		},

		saveQuote: async function() {
			if (this.form.id) {
				await axios.put(this.route, this.form);
			} else {
				await axios.post(this.route, this.form);
			}
		},

		// Upload Json
		fromJson: async function() {
			await axios.post(`${this.route}/json`, JSON.parse(this.json));
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