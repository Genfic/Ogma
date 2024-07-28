import {
	DeleteApiQuotes as deleteQuote,
	GetAllQuotes as getAllQuotes,
	PostApiQuotes as createQuote,
	PostApiQuotesJson as createQuotesFromJson,
	PutApiQuotes as updateQuote,
} from "../../generated/paths-public";

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

		editorOpen: false
	},
	methods: {
		// Gets all existing namespaces
		getQuotes: async function() {
			const data = await getAllQuotes();
			this.quotes = await data.json();
		},

		deleteQuote: async function(q) {
			if (confirm("Delete permanently?")) {
				const data = await deleteQuote(q.id);
				const id = await data.json();
				this.quotes = this.quotes.filter((i) => i.id !== id);
			}
		},

		openEditor: function(q) {
			this.editorOpen = true;
			this.form = q;
		},

		closeEditor: function() {
			this.editorOpen = false;
			for (const key in Object.keys(this.form)) {
				this.form[key] = null;
			}
		},

		saveQuote: async function() {
			if (this.form.id) {
				await updateQuote(this.form);
			} else {
				await createQuote(this.form);
			}
		},

		// Upload Json
		fromJson: async function() {
			const res = await createQuotesFromJson(this.json);
			alert(`Created ${await res.json()} quotes`);
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
