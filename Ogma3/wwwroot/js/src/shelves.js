let shelves_vue = new Vue({ 
    el: "#shelves",
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
        err: null,
        shelves: [],
        route: null,
        owner: null
    },
    methods: {

        // Contrary to its name, it also modifies a shelf if needed.
        // It was simply easier to slap both functionalities into a single function.
        createShelf: function(e) {
            e.preventDefault();
                        
            // Validation
            this.err = [];
            if (this.form.name.length > this.lens.maxNameLength || this.form.name.length < this.lens.minNameLength) 
                this.err.push(`Name has to be between ${this.lens.minNameLength} and ${this.lens.maxNameLength} characters long.`);
            if (this.form.desc && this.form.desc.length > this.lens.maxDescLength) 
                this.err.push(`Description has to be less than ${this.lens.maxDescLength} characters long.`);
            if (this.err.length > 0) return;
            

            if (this.form.name) {

                let shelf = {
                    name: this.form.name,
                    description: this.form.desc,
                    isPublic: this.form.pub,
                    isQuick: this.form.quick,
                    color: this.form.color,
                    icon: Number(this.form.icon)
                }
                
                // If no ID has been set, that means it's a new shelf.
                // Thus, we POST it.
                if (this.form.id === null) {
                    axios.post(this.route, shelf,
                        { 
                            headers: { "RequestVerificationToken" : this.csrf } 
                        })
                        .then(_ => {
                            this.cancelEdit();
                            this.getShelves()
                        })
                        .catch(console.error); 
                    
                // If the ID is set, that means it's an existing shelf.
                // Thus, we PUT it.
                } else {
                    shelf.id = this.form.id;
                    axios.put(`${this.route}/${this.form.id}`, shelf,
                        {
                            headers: { "RequestVerificationToken" : this.csrf }
                        })
                        .then(_ => {
                            this.getShelves()
                        })
                        .catch(console.error)
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
            axios.get(`${this.route}/user/${this.owner}`)
                .then(response => {
                    if (response.status === 200) {
                        this.shelves = response.data
                    } else {
                        this.shelves = null;
                    }
                })
                .catch(console.error);
        },

        // Deletes a selected shelf
        deleteShelf: function(t) {
            if (confirm("Delete permanently?")) {
                axios.delete(`${this.route}/${t.id}`,
                    { headers: { "RequestVerificationToken" : this.csrf }})
                    .then(_ => {
                        this.getShelves()
                    })
                    .catch(console.error);
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
            this.form.icon  = t.iconId;
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
        // CSRF token
        this.csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
        // Grab the route from route helper
        this.route = document.getElementById('route').dataset.route;
        // Get owner
        this.owner = document.getElementById('owner').dataset.owner;
        // Get validation
        axios.get(`${this.route}/validation`)
            .then(r => {
                this.lens = r.data;
            })
            .catch(e => console.error(e));
        // Grab the initial set of shelves
        this.getShelves();
    }
});