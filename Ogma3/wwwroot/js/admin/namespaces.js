let anamespaces_vue = new Vue({
    el: "#app",
    data: {
        form: {
            name: null,
            color: {
                a: null,
                r: null,
                g: null,
                b: null,
            },
            id: null
        },
        lens: {
            minNameLength: 5,
            maxNameLength: 10,
        },
        err: [],
        namespaces: [],
        route: null,
        color: null
    },
    methods: {

        // Contrary to its name, it also modifies a namespace if needed.
        // It was simply easier to slap both functionalities into a single function.
        createNamespace: function (e) {
            e.preventDefault();

            // Validation
            this.err = [];
            if (this.form.name.length > this.lens.maxNameLength || this.form.name.length < this.lens.minNameLength)
                this.err.push(`Name has to be between ${this.lens.minNameLength} and ${this.lens.maxNameLength} characters long.`);
            if (this.err.length > 0) return;

            if (this.form.name) {

                // If no ID has been set, that means it's a new namespace.
                // Thus, we POST it.
                if (this.form.id === null) {
                    axios.post(this.route,
                        {
                            name: this.form.name,
                            argb: calculateColor(this.form.color)
                        })
                        .then(_ => {
                            this.getNamespaces()
                        })
                        .catch(error => {
                            console.log(error);
                        });

                    // If the ID is set, that means it's an existing namespace.
                    // Thus, we PUT it.
                } else {
                    axios.put(this.route + '/' + this.form.id,
                        {
                            id: this.form.id,
                            name: this.form.name,
                            argb: calculateColor(this.form.color)
                        })
                        .then(_ => {
                            this.getNamespaces()
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
        getNamespaces: function () {
            axios.get(this.route)
                .then(response => {
                    this.namespaces = response.data
                })
                .catch(error => {
                    console.log(error);
                });
        },

        // Deletes a selected namespace
        deleteNamespace: function (t) {
            axios.delete(this.route + '/' + t.id)
                .then(_ => {
                    this.getNamespaces()
                })
                .catch(error => {
                    console.log(error);
                });
        },

        // Throws a namespace from the list into the editor
        editNamespace: function (t) {
            this.form.name = t.name;
            this.form.color = t.color;
            this.form.color.a = t.color.a > 0 ? (t.color.a / 255).toFixed(2) : 0; // normalize alpha
            this.form.id = t.id;
            this.updateColor();
        },

        // Clears the editor
        cancelEdit: function () {
            this.form.name =
                this.form.color =
                    this.form.id = null;
        },
        
        // Update the color
        updateColor: function () {
            this.color = `rgba(${this.form.color.r}, ${this.form.color.g}, ${this.form.color.b}, ${this.form.color.a})`;
        }
    }, 

    mounted() {
        // Grab the route from route helper
        this.route = document.getElementById('route').dataset.route;
        // Get validation data
        axios.get(this.route + '/validation')
            .then(r => {
                this.lens = r.data;
            })
            .catch(e => console.error(e));
        // Grab the initial set of namespaces
        this.getNamespaces();
    }
});

function calculateColor (color) {
    let a = Math.round(color.a * 255).toString(16);
    let r = color.r.toString(16);
    let g = color.g.toString(16);
    let b = color.b.toString(16);
    return parseInt(`${a}${r}${g}${b}`, 16)
}