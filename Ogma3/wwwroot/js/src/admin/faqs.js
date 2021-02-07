new Vue({
    el: '#faqs',
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
        createFaq: function (e) {
            console.log(e)
            e.preventDefault();

            if (this.form.question && this.form.answer) {
                
                let data = {
                    question: this.form.question,
                    answer: this.form.answer
                }

                // If no ID has been set, that means it's a new rating.
                // Thus, we POST it.
                if (this.form.id === null) {
                    axios.post(this.route, data, {
                        headers: {'RequestVerificationToken': this.xcsrf}
                    })
                        .then(_ => {
                            this.getFaqs()
                        })
                        .catch(console.error);

                    // If the ID is set, that means it's an existing namespace.
                    // Thus, we PUT it.
                } else {
                    data.id = this.form.id;
                    
                    axios.put(`${this.route}/${this.form.id}`, data, {
                        headers: {'RequestVerificationToken': this.xcsrf}
                    })
                        .then(_ => {
                            this.getFaqs()
                        })
                        .catch(console.error)
                        .then(_ => this.cancelEdit());
                }

            }
        },

        // Gets all existing namespaces
        getFaqs: function () {
            axios.get(this.route)
                .then(res => {
                    this.faqs = res.data
                })
                .catch(console.error);
        },

        // Deletes a selected namespace
        deleteFaq: function (t) {
            if (confirm("Delete permanently?")) {
                axios.delete(`${this.route}/${t.id}`, {
                    headers: {'RequestVerificationToken': this.xcsrf}
                })
                    .then(_ => {
                        this.getFaqs()
                    })
                    .catch(console.error);
            }
        },

        // Throws a faq from the list into the editor
        editFaq: function (t) {
            this.form.question = t.question;
            this.form.answer = t.answer;
            this.form.id = t.id;
        },

        // Clears the editor
        cancelEdit: function () {
            this.form.question =
                this.form.answer =
                    this.form.id = null;
        },
    },
    mounted() {
        this.route = document.getElementById('route').dataset.route;
        this.xcsrf = document.querySelector('[name="__RequestVerificationToken"]').value;
        this.getFaqs()
    }
})