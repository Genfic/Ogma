let vue = new Vue({ 
    el: "#app",
    data: {
        form: {
            name: null,
            desc: null,
            id: null
        },
        tags: [],
        route: null
    },
    methods: {

        // Contrary to its name, it also modifies a tag if needed.
        // It was simply easier to slap both functionalities into a single function.
        createTag: function(e) {
            e.preventDefault();

            if (this.form.name && this.form.desc) {

                // If no ID has been set, that means it's a new tag.
                // Thus, we POST it.
                if (this.form.id === null) {
                    axios.post(this.route,
                        {
                            name: this.form.name,
                            description: this.form.desc
                        })
                        .then(_ => {
                            this.getTags()
                        })
                        .catch(error => {
                            console.log(error);
                        });
                    
                // If the ID is set, that means it's an existing tag.
                // Thus, we PUT it.
                } else {
                    axios.put(this.route + '/' + this.form.id,
                        {
                            id: this.form.id,
                            name: this.form.name,
                            description: this.form.desc
                        })
                        .then(_ => {
                            this.getTags()
                        })
                        .catch(error => {
                            console.log(error);
                        })
                        // Clear the form too
                        .then(_ => {
                            this.form.name =
                                this.form.desc =
                                    this.form.id = null;
                        });
                }

            }
        },

        // Gets all existing tags
        getTags: function() {
            axios.get(this.route)
                .then(response => {
                    this.tags = response.data
                })
                .catch(error => {
                    console.log(error);
                });
        },

        // Deletes a selected tag
        deleteTag: function(t) {
            axios.delete(this.route + '/' + t.id) 
                .then(_ => {
                    this.getTags() 
                })
                .catch(error => {
                    console.log(error);
                });
        },

        // Throws a tag from the list into the editor
        editTag: function(t) {
            this.form.name = t.name;
            this.form.desc = t.description;
            this.form.id = t.id;
        },

        // Clears the editor
        cancelEdit: function() {
            this.form.name =
                this.form.desc =
                    this.form.id = null;
        }
    },
    
    mounted() {
        // Grab the route from route helper
        this.route = document.getElementById('route').dataset.route;
        // Grab the initial set of tags
        this.getTags();
    }
});