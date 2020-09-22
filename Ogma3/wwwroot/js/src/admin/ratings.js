let ratings_vue = new Vue({
    el: "#app",
    data: {
        form: {
            name: null,
            desc: null,
            icon: null,
            id: null
        },
        err: [],
        ratings: [],
        route: null,
        cdn: null,
        xcsrf: null,
    },
    methods: {
        iconChanged: function(e) {
            this.form.icon = e.target.files[0];
        },

        // Contrary to its name, it also modifies a namespace if needed.
        // It was simply easier to slap both functionalities into a single function.
        createRating: function (e) {
            e.preventDefault();
            
            if (this.form.name) {

                const data = new FormData();

                data.append('name', this.form.name);
                data.append('description', this.form.desc);
                if (this.form.icon)
                    data.append('icon', this.form.icon, this.form.icon.name)
                
                // If no ID has been set, that means it's a new rating.
                // Thus, we POST it.
                if (this.form.id === null) {
                    axios.post(this.route, data, { headers: { 'RequestVerificationToken': this.xcsrf } })
                        .then(_ => {
                            this.getRatings()
                        })
                        .catch(error => {
                            console.log(error);
                        });

                    // If the ID is set, that means it's an existing namespace.
                    // Thus, we PUT it.
                } else {                    
                    axios.put(this.route + '/' + this.form.id, data, { headers: { 'RequestVerificationToken': this.xcsrf } })
                        .then(_ => {
                            this.getRatings()
                        })
                        .catch(error => {
                            console.log(error);
                        })
                        // Clear the form too
                        .then(_ => {
                            this.form.name =
                                this.form.id = null;
                        });
                }

            }
        },

        // Gets all existing namespaces
        getRatings: function () {
            axios.get(this.route)
                .then(response => {
                    this.ratings = response.data
                })
                .catch(error => {
                    console.log(error);
                });
        },

        // Deletes a selected namespace
        deleteRating: function (t) {
            if(confirm("Delete permanently?")) {
                axios.delete(this.route + '/' + t.id)
                    .then(_ => {
                        this.getRatings()
                    })
                    .catch(error => {
                        console.log(error);
                    });
            }
        },

        // Throws a namespace from the list into the editor
        editRating: function (t) {
            this.form.name = t.name;
            this.form.color = t.color;
            this.form.id = t.id;
            this.form.order = t.order;
        },

        // Clears the editor
        cancelEdit: function () {
            this.form.name =
                this.form.color =
                    this.form.id = 
                        this.form.order = null;
        },
    }, 

    mounted() {
        // Grab the route from route helper
        this.route = document.getElementById('route').dataset.route;
        this.cdn   = document.getElementById('cdn').dataset.cdn;
        this.xcsrf = document.querySelector('[name="__RequestVerificationToken"]').value;
        // Grab the initial set of namespaces
        this.getRatings();
    }
});