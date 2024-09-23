import {
	DeleteApiQuotes as deleteQuote,
	GetAllQuotes as getAllQuotes,
	PostApiQuotes as createQuote,
	PostApiQuotesJson as createQuotesFromJson,
	PutApiQuotes as updateQuote,
} from "../../generated/paths-public";
import type { QuoteDto } from "../../generated/types-public";

type Quote = QuoteDto & { id: number };

// @ts-ignore
new Vue({
	el: "#app",
	data: {
		form: {
			id: null,
			body: null,
			author: null,
		} as Quote,
		quotes: [] as QuoteDto[],
		json: null,
		search: "",

		editorOpen: false,
	},
	methods: {
		// Gets all existing namespaces
		getQuotes: async function () {
			const data = await getAllQuotes();
			this.quotes = await data.json();
		},

		deleteQuote: async function (q: Quote) {
			if (confirm("Delete permanently?")) {
				const data = await deleteQuote(q.id);
				const id = await data.json();
				this.quotes = this.quotes.filter((i) => i.id !== id);
			}
		},

		openEditor: function (q: Quote) {
			this.editorOpen = true;
			this.form = q;
		},

		closeEditor: function () {
			this.editorOpen = false;
			for (const key in Object.keys(this.form)) {
				this.form[key] = null;
			}
		},

		saveQuote: async function () {
			if (this.form.id) {
				await updateQuote(this.form);
			} else {
				await createQuote(this.form);
			}
		},

		// Upload Json
		fromJson: async function () {
			const res = await createQuotesFromJson({ quotes: this.json });
			alert(`Created ${await res.json()} quotes`);
		},
	},

	watch: {
		search() {
			for (const q of this.quotes) {
				q.show = this.search
					? q.body.toLowerCase().includes(this.search.toLowerCase()) || q.author.toLowerCase().includes(this.search.toLowerCase())
					: true;
			}
		},
	},

	async mounted() {
		await this.getQuotes();
	},
});
