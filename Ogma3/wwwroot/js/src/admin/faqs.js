new Vue({
	el: "#faqs",
	data: {
		form: {
			question: null,
			answer: null,
			id: null
		},
		faqs: [],
		route: null,
		xcsrf: null
	},
	methods: {

		// Contrary to its name, it also modifies a namespace if needed.
		// It was simply easier to slap both functionalities into a single function.
		createFaq: async function(e) {
			log.log(e);
			e.preventDefault();

			if (this.form.question && this.form.answer) {

				const data = {
					question: this.form.question,
					answer: this.form.answer
				};
				
				const options = {
					headers: { "RequestVerificationToken": this.xcsrf }
				};

				// If no ID has been set, that means it's a new rating.
				// Thus, we POST it.
				if (this.form.id === null) {
					await axios.post(this.route, data, options);
					await this.getFaqs();

					// If the ID is set, that means it's an existing namespace.
					// Thus, we PUT it.
				} else {
					await axios.put(this.route, { id: this.form.id, ...data }, options);
					await this.getFaqs();
					this.cancelEdit();
				}

			}
		},

		// Gets all existing namespaces
		getFaqs: async function() {
			const { data } = await axios.get(this.route);
			this.faqs = data;
		},

		// Deletes a selected namespace
		deleteFaq: async function(t) {
			if (confirm("Delete permanently?")) {
				await axios.delete(`${this.route}/${t.id}`, {
					headers: { "RequestVerificationToken": this.xcsrf }
				});
				await this.getFaqs();
			}
		},

		// Throws a faq from the list into the editor
		editFaq: function(t) {
			this.form.question = t.question;
			this.form.answer = t.answer;
			this.form.id = t.id;
		},

		// Clears the editor
		cancelEdit: function() {
			this.form.question =
				this.form.answer =
					this.form.id = null;
		}
	},
	async mounted() {
		this.route = document.getElementById("route").dataset.route;
		this.xcsrf = document.querySelector("[name=__RequestVerificationToken]").value;
		await this.getFaqs();
	}
});