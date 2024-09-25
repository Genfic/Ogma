import {
	PostApiFaqs as createFaq,
	DeleteApiFaqs as deleteFaq,
	GetApiFaqs as getAllFaqs,
	PutApiFaqs as updateFaq,
} from "../../generated/paths-public";
import type { FaqDto } from "../../generated/types-public";
import { log } from "../../src-helpers/logger";

// @ts-ignore
new Vue({
	el: "#faqs",
	data: {
		form: {
			question: null,
			answer: null,
			id: null,
		},
		faqs: [] as FaqDto[],
		route: null,
		xcsrf: null,
	},
	methods: {
		// Contrary to its name, it also modifies a namespace if needed.
		// It was simply easier to slap both functionalities into a single function.
		createFaq: async function (e) {
			log.log(e);
			e.preventDefault();

			if (this.form.question && this.form.answer) {
				const data = {
					question: this.form.question,
					answer: this.form.answer,
				};

				const headers = { RequestVerificationToken: this.xcsrf };

				if (this.form.id === null) {
					// If no ID has been set, that means it's a new rating.
					// Thus, we POST it.
					const res = await createFaq(data, headers);
					if (res.ok) {
						await this.getFaqs();
					}
				} else {
					// If the ID is set, that means it's an existing namespace.
					// Thus, we PUT it.
					const res = await updateFaq({ id: this.form.id, ...data }, headers);
					if (res.ok) {
						await this.getFaqs();
						this.cancelEdit();
					}
				}
			}
		},

		// Gets all existing namespaces
		getFaqs: async function () {
			const res = await getAllFaqs();
			if (res.ok) {
				this.faqs = await res.json();
			}
		},

		// Deletes a selected namespace
		deleteFaq: async function (t: FaqDto) {
			if (confirm("Delete permanently?")) {
				const res = await deleteFaq(t.id, { RequestVerificationToken: this.xcsrf });
				if (res.ok) {
					await this.getFaqs();
				}
			}
		},

		// Throws a faq from the list into the editor
		editFaq: function (t: FaqDto) {
			this.form.question = t.question;
			this.form.answer = t.answer;
			this.form.id = t.id;
		},

		// Clears the editor
		cancelEdit: function () {
			this.form.question = this.form.answer = this.form.id = null;
		},
	},
	async mounted() {
		this.xcsrf = (document.querySelector("[name=__RequestVerificationToken]") as HTMLInputElement).value;
		await this.getFaqs();
	},
});
