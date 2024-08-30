import {
	DeleteApiShelves as deleteShelf,
	GetApiShelves as getShelves,
	PostApiShelves as createShelf,
	PutApiShelves as updateShelf,
} from "../generated/paths-public";
import type { ShelfDto } from "../generated/types-public";

// @ts-ignore
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
			id: null,
		},
		showForm: false,
		csrf: null,
		err: null,
		shelves: [] as ShelfDto[],
		route: null,
		owner: null,
	},
	methods: {
		// Contrary to its name, it also modifies a shelf if needed.
		// It was simply easier to slap both functionalities into a single function.
		createShelf: async function (e: Event) {
			e.preventDefault();

			if (this.form.name) {
				const shelf = {
					name: this.form.name,
					description: this.form.desc,
					isPublic: this.form.pub,
					isQuickAdd: this.form.quick,
					trackUpdates: this.form.track,
					color: this.form.color,
					icon: Number(this.form.icon),
				};

				const headers = { RequestVerificationToken: this.csrf };

				if (this.form.id === null) {
					// If no ID has been set, that means it's a new shelf.
					// Thus, we POST it.
					const res = await createShelf(shelf, headers);
					if (res.ok) {
						await this.getShelves();
						this.cancelEdit();
					}
				} else {
					// If the ID has been set, that means it's an existing shelf.
					// Thus, we PUT it.
					const res = await updateShelf({ id: this.form.id, ...shelf }, headers);
					if (res.ok) {
						await this.getShelves();
						this.cancelEdit();
						this.showForm = false;
					}
				}
			}
		},
		// Gets all existing shelves
		getShelves: async function () {
			const res = await getShelves(this.owner, 1);
			if (res.ok) {
				this.shelves = await res.json();
			}
		},

		// Deletes a selected shelf
		deleteShelf: async function (t: ShelfDto) {
			if (confirm("Delete permanently?")) {
				const res = await deleteShelf(t.id, { RequestVerificationToken: this.csrf });
				if (res.ok) {
					await this.getShelves();
				}
			}
		},

		// Throws a shelf from the list into the editor
		editShelf: function (t: ShelfDto) {
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
			for (const key of Object.keys(this.form)) {
				this.form[key] = null;
			}
			this.form.isQuickAdd = this.form.isPublic = false;
			this.showForm = false;
		},
	},

	async mounted() {
		// CSRF token
		this.csrf = (document.querySelector("input[name=__RequestVerificationToken]") as HTMLInputElement).value;

		// Get owner
		this.owner = document.getElementById("owner").dataset.owner;

		// Grab the initial set of shelves
		await this.getShelves();
	},
});
