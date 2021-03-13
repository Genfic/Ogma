let atags_vue = new Vue({ 
	el: "#app",
	data: {
		form: {
			name: null,
			desc: null,
			namespace: null,
			id: null
		},
		lens: {
			minNameLength:5,
			maxNameLength:20,
			minDescLength:10,
			maxDescLength:100
		},
		err: [],
		routes: {
			tags: null,
		},
		tags: [],
	},
	methods: {

		// Contrary to its name, it also modifies a tag if needed.
		// It was simply easier to slap both functionalities into a single function.
		createTag: function(e) {
			e.preventDefault();

			// Validation
			this.err = [];
			if (this.form.name.length > this.lens.maxNameLength || this.form.name.length < this.lens.minNameLength)
				this.err.push(`Name has to be between ${this.lens.minNameLength} and ${this.lens.maxNameLength} characters long.`);
			if (this.form.desc && this.form.desc.length > this.lens.maxDescLength)
				this.err.push(`Description has to be at most ${this.lens.maxDescLength} characters long.`);
			if (this.err.length > 0) return; 

			if (this.form.name) {

				// If no ID has been set, that means it's a new tag.
				// Thus, we POST it.
				if (this.form.id === null) {
					axios.post(this.routes.tags,
						{
							name: this.form.name,
							namespace: Number(this.form.namespace),
							description: this.form.desc
						}, {
							headers: { 'RequestVerificationToken' : this.csrf }
						})
						.then(() => {
							this.getTags();
						})
						.catch(error => {
							console.log(error);
						});
                    
					// If the ID is set, that means it's an existing tag.
					// Thus, we PUT it.
				} else {
					axios.put(this.routes.tags + '/' + this.form.id,
						{
							id: this.form.id,
							name: this.form.name,
							namespace: Number(this.form.namespace),
							description: this.form.desc
						}, {
							headers: { 'RequestVerificationToken' : this.csrf }
						})
						.then(() => {
							this.getTags();
						})
						.catch(error => {
							console.log(error);
						})
					// Clear the form too
						.then(() => {
							this.form.name =
                                this.form.desc =
                                    this.form.namespace = 
                                        this.form.id = null;
						});
				}

			}
		},

		// Gets all existing tags
		getTags: function() {
			axios.get(this.routes.tags + '/all')
				.then(response => {
					this.tags = response.data;
				})
				.catch(error => {
					console.log(error);
				});
		},

		// Deletes a selected tag
		deleteTag: function(t) {
			if(confirm("Delete permanently?")) {
				axios.delete(this.routes.tags + '/' + t.id)
					.then(() => {
						this.getTags();
					})
					.catch(error => {
						console.log(error);
					});
			}
		},

		// Throws a tag from the list into the editor
		editTag: function(t) {
			this.form.name = t.name;
			this.form.desc = t.description;
			this.form.namespace = t.namespaceId;
			this.form.id = t.id;
		},

		// Clears the editor
		cancelEdit: function() {
			this.form.name =
                this.form.desc =
                    this.form.namespace =
                        this.form.id = null;
		}
	},
    
	mounted() {
		this.csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
		// Grab the routes from route helpers
		this.routes.tags = document.getElementById('tag-route').dataset.route;
		// Get validation data
		axios.get(this.routes.tags + '/validation') 
			.then(r => {
				this.lens = r.data;
			})
			.catch(e => console.error(e));
		// Grab the initial set of tags
		this.getTags();
	}
});