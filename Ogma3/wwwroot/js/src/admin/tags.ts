// @ts-ignore
import { TagDto } from "../../generated/types-public";
import { DeleteApiTags, GetApiTagsAll, PostApiTags, PutApiTags } from "../../generated/paths-public";

interface Validation {
	minNameLength: number;
	maxNameLength: number;
	maxDescLength: number;
}

// @ts-ignore
new Vue({
	el: "#app",
	data: {
		form: {
			name: null as string,
			desc: null as string,
			namespace: null as string,
			id: null as number,
		},
		lens: null as Validation|null,
		err: [],
		tags: [] as TagDto[],
	},
	methods: {
		// Contrary to its name, it also modifies a tag if needed.
		// It was simply easier to slap both functionalities into a single function.
		createTag: async function (e: Event) {
			e.preventDefault();

			// Validation
			this.err = [];
			if (this.form.name.length > this.lens.maxNameLength || this.form.name.length < this.lens.minNameLength)
				this.err.push(
					`Name has to be between ${this.lens.minNameLength} and ${this.lens.maxNameLength} characters long.`,
				);
			if (this.form.desc && this.form.desc.length > this.lens.maxDescLength)
				this.err.push(`Description has to be at most ${this.lens.maxDescLength} characters long.`);
			if (this.err.length > 0) return;

			if (this.form.name) {
				const body = {
					name: this.form.name,
					namespace: Number(this.form.namespace),
					description: this.form.desc,
				};

				const headers = { RequestVerificationToken: this.csrf };

				if (this.form.id === null) {
					// If no ID has been set, that means it's a new tag.
					// Thus, we POST it.
					const res = await PostApiTags(body, headers);
					if (res.ok) {
						await this.getTags();
					}
				} else {
					// If the ID is set, that means it's an existing tag.
					// Thus, we PUT it.
					const res = await PutApiTags({ id: this.form.id, ...body }, headers);
					if (res.ok) {
						await this.getTags();
						// Clear the form too
						this.form.name = this.form.desc = this.form.namespace = this.form.id = null;
					}
				}
			}
		},

		// Gets all existing tags
		getTags: async function () {
			const res = await GetApiTagsAll();
			this.tags = await res.json();
		},

		// Deletes a selected tag
		deleteTag: async function (t: TagDto) {
			if (confirm("Delete permanently?")) {
				const res = await DeleteApiTags(t.id);
				if (res.ok) {
					await this.getTags();
				}
			}
		},

		// Throws a tag from the list into the editor
		editTag: function (t: TagDto) {
			this.form.name = t.name;
			this.form.desc = t.description;
			this.form.namespace = t.namespaceId;
			this.form.id = t.id;
		},

		// Clears the editor
		cancelEdit: function () {
			this.form.name = this.form.desc = this.form.namespace = this.form.id = null;
		},
	},

	async mounted() {
		this.csrf = (document.querySelector("input[name=__RequestVerificationToken]") as HTMLInputElement).value;

		const { Validation } = JSON.parse(document.getElementById("static-data").innerText);
		this.lens = Validation;

		await this.getTags();
	},
});
