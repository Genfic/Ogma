let shelves_vue = new Vue({ 
    el: "#app",
    data: {
        form: {
            name: null,
            desc: null,
            pub: false,
            quick: false,
            color: null,
            icon: null,
            id: null
        },
        lens: {
            minNameLength:3,
            maxNameLength:50,
            maxDescLength:255
        },
        showForm: false,
        csrf: null,
        err: [],
        shelves: [],
        route: null,
        owner: null
    },
    methods: {

        // Contrary to its name, it also modifies a shelf if needed.
        // It was simply easier to slap both functionalities into a single function.
        createShelf: function(e) {
            e.preventDefault();
            
            // Set CSRF
            this.csrf = document.querySelector('input[name=__RequestVerificationToken').value;
            
            // Validation
            this.err = [];
            if (this.form.name.length > this.lens.maxNameLength || this.form.name.length < this.lens.minNameLength) 
                this.err.push(`Name has to be between ${this.lens.minNameLength} and ${this.lens.maxNameLength} characters long.`);
            if (this.form.desc && this.form.desc.length > this.lens.maxDescLength) 
                this.err.push(`Description has to be less than ${this.lens.maxDescLength} characters long.`);
            if (this.err.length > 0) return;
            

            if (this.form.name) {

                // If no ID has been set, that means it's a new shelf.
                // Thus, we POST it.
                if (this.form.id === null) {
                    axios.post(this.route,
                        {
                            name: this.form.name,
                            description: this.form.desc,
                            isPublic: this.form.pub,
                            isQuick: this.form.quick,
                            color: this.form.color,
                            icon: Number(this.form.icon)
                        },{
                            headers: { "RequestVerificationToken" : this.csrf }
                        })
                        .then(_ => {
                            this.getShelves()
                        })
                        .catch(error => {
                            console.log(error);
                        }); 
                    
                // If the ID is set, that means it's an existing shelf.
                // Thus, we PUT it.
                } else {
                    axios.put(this.route + '/' + this.form.id,
                        {
                            id: this.form.id,
                            name: this.form.name, 
                            description: this.form.desc,
                            isPublic: this.form.pub,
                            isQuick: this.form.quick,
                            color: this.form.color,
                            shelf: this.form.shelf,
                            icon: Number(this.form.icon)
                        },{
                            headers: { "RequestVerificationToken" : this.csrf }
                        })
                        .then(_ => {
                            this.getShelves()
                        })
                        .catch(error => {
                            console.log(error);
                        })
                        // Clear the form too
                        .then(_ => {
                            this.cancelEdit();
                            this.showForm = false;
                        });
                }

            }
        },
        // Gets all existing shelves
        getShelves: function() {
            axios.get(this.route + '/user/' + this.owner)
                .then(response => {
                    this.shelves = response.data
                })
                .catch(error => {
                    console.log(error);
                });
        },

        // Deletes a selected shelf
        deleteShelf: function(t) {
            if (confirm("Delete permanently?")) {
                axios.delete(this.route + '/' + t.id,
                    null,
                    {
                        headers: { "RequestVerificationToken" : this.csrf }
                    })
                    .then(_ => {
                        this.getShelves()
                    })
                    .catch(error => {
                        console.log(error);
                    });
            }
        },

        // Throws a shelf from the list into the editor
        editShelf: function(t) {
            this.form.name  = t.name;
            this.form.desc  = t.description;
            this.form.id    = t.id;
            this.form.color = t.color;
            this.form.quick = t.isQuick;
            this.form.pub   = t.isPublic;
            this.showForm   = true;
        },

        // Clears the editor
        cancelEdit: function() {
            this.form.name =
                this.form.desc =
                    this.form.id = 
                        this.form.color = null;
            this.form.isQuick = 
                this.form.isPublic = false;
        },
    },
    
    mounted() {
        // Grab the route from route helper
        this.route = document.getElementById('route').dataset.route;
        // Get owner
        this.owner = document.getElementById('owner').dataset.owner;
        // Get validation
        axios.get(this.route + '/validation')
            .then(r => {
                this.lens = r.data;
            })
            .catch(e => console.error(e));
        // Grab the initial set of shelves
        this.getShelves();
    }
});