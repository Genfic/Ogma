let vue = new Vue({ 
    el: "#app",
    data: {
        form: {
            name: null,
            desc: null,
            id: null
        },
        lens: {
            minNameLength:5,
            maxNameLength:20,
            minDescLength:10,
            maxDescLength:100
        },
        err: [],
        categories: [],
        route: null
    },
    methods: {

        // Contrary to its name, it also modifies a category if needed.
        // It was simply easier to slap both functionalities into a single function.
        createCategory: function(e) {
            e.preventDefault();
            
            
            // Validation
            this.err = [];
            if (this.form.name.length > this.lens.maxNameLength || this.form.name.length < this.lens.minNameLength) 
                this.err.push(`Name has to be between ${this.lens.minNameLength} and ${this.lens.maxNameLength} characters long.`);
            if (this.form.desc.length > this.lens.maxDescLength || this.form.desc.length < this.lens.minDescLength) 
                this.err.push(`Description has to be between ${this.lens.minDescLength} and ${this.lens.maxDescLength} characters long.`);
            if (this.err.length > 0) return;
            

            if (this.form.name && this.form.desc) {

                // If no ID has been set, that means it's a new category.
                // Thus, we POST it.
                if (this.form.id === null) {
                    axios.post(this.route,
                        {
                            name: this.form.name,
                            description: this.form.desc
                        })
                        .then(_ => {
                            this.getCategories()
                        })
                        .catch(error => {
                            console.log(error);
                        });
                    
                // If the ID is set, that means it's an existing category.
                // Thus, we PUT it.
                } else {
                    axios.put(this.route + '/' + this.form.id,
                        {
                            id: this.form.id,
                            name: this.form.name,
                            description: this.form.desc
                        })
                        .then(_ => {
                            this.getCategories()
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

        // Gets all existing categories
        getCategories: function() {
            axios.get(this.route)
                .then(response => {
                    this.categories = response.data
                })
                .catch(error => {
                    console.log(error);
                });
        },

        // Deletes a selected category
        deleteCategory: function(t) {
            axios.delete(this.route + '/' + t.id) 
                .then(_ => {
                    this.getCategories() 
                })
                .catch(error => {
                    console.log(error);
                });
        },

        // Throws a category from the list into the editor
        editCategory: function(t) {
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
        // Get validation
        axios.get(this.route + '/validation')
            .then(r => {
                this.lens = r.data;
            })
            .catch(e => console.error(e));
        // Grab the initial set of categories
        this.getCategories();
    }
});