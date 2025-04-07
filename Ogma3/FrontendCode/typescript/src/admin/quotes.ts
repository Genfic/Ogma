import {
	PostApiQuotes as createQuote,
	PostApiQuotesJson as createQuotesFromJson,
	DeleteApiQuotes as deleteQuote,
	GetAllQuotes as getAllQuotes,
	PutApiQuotes as updateQuote,
} from "@g/paths-public";
import type { FullQuoteDto } from "@g/types-public";

// @ts-ignore
new Vue({
	el: "#app",
	data: {
		form: {
			id: null,
			body: null,
			author: null,
		} as FullQuoteDto,
		quotes: [] as FullQuoteDto[],
		json: null,
		search: "",

		editorOpen: false,
	},
	methods: {
		// Gets all existing namespaces
		getQuotes: async function () {
			const data = await getAllQuotes();
			this.quotes = data.data;
		},

		deleteQuote: async function (q: FullQuoteDto) {
			if (confirm("Delete permanently?")) {
				const data = await deleteQuote(q.id);
				const id = data.data;
				this.quotes = this.quotes.filter((i: FullQuoteDto) => i.id !== id);
			}
		},

		openEditor: function (q: FullQuoteDto) {
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
			alert(`Created ${res.data} quotes`);
		},
	},

	watch: {
		search() {
			for (const q of this.quotes) {
				q.show = this.search
					? q.body.toLowerCase().includes(this.search.toLowerCase()) ||
						q.author.toLowerCase().includes(this.search.toLowerCase())
					: true;
			}
		},
	},

	async mounted() {
		await this.getQuotes();
	},
});
