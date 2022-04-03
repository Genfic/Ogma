new Vue({
    el: "#shelves",
    data: {
        form: {
            name: null,
            desc: null,

            pub: false,
            quick: false,
            track: false,

            color: null,
            icon: null,
            id: null
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
        createShelf: async function (e) {
            e.preventDefault();

            if (this.form.name) {

                const shelf = {
                    name: this.form.name,
                    description: this.form.desc,
                    isPublic: this.form.pub,
                    isQuickAdd: this.form.quick,
                    trackUpdates: this.form.track,
                    color: this.form.color,
                    icon: Number(this.form.icon)
                };

                const options = {
                    headers: {"RequestVerificationToken": this.csrf}
                };

                // If no ID has been set, that means it's a new shelf.
                // Thus, we POST it.
                if (this.form.id === null) {
                    await axios.post(this.route, shelf, options);

                    await this.getShelves();
                    this.cancelEdit();

                    // If the ID has been set, that means it's an existing shelf.
                    // Thus, we PUT it.
                } else {
                    shelf.id = this.form.id;
                    await axios.put(this.route, {id: this.form.id, ...shelf}, options);

                    await this.getShelves();
                    this.cancelEdit();
                    this.showForm = false;
                }

            }
        },
        // Gets all existing shelves
        getShelves: async function () {
            const {data, status} = await axios.get(`${this.route}/${this.owner}?page=1`);
            this.shelves = status === 200
                ? data
                : null;

        },

        // Deletes a selected shelf
        deleteShelf: async function (t) {
            if (confirm("Delete permanently?")) {
                await axios.delete(`${this.route}/${t.id}`,
                    {headers: {"RequestVerificationToken": this.csrf}}
                );

                await this.getShelves();
            }
        },

        // Throws a shelf from the list into the editor
        editShelf: function (t) {
            this.form.name = t.name;
            this.form.desc = t.description;
            this.form.id = t.id;
            this.form.color = t.color;
            this.form.quick = t.isQuickAdd;
            this.form.pub = t.isPublic;
            this.form.track = t.trackUpdates;
            this.form.icon = t.iconId;
            this.showForm = true;
        },

        // Clears the editor
        cancelEdit: function () {
            Object.keys(this.form).forEach((i) => this.form[i] = null);
            this.form.isQuickAdd = this.form.isPublic = false;
            this.showForm = false;
        }
    },

    async mounted() {
        // CSRF token
        this.csrf = document.querySelector("input[name=__RequestVerificationToken]").value;
        // Grab the route from route helper
        this.route = document.getElementById("route").dataset.route;
        // Get owner
        this.owner = document.getElementById("owner").dataset.owner;

        // Grab the initial set of shelves
        await this.getShelves();
    }
});