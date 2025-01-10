import { DeleteAdminApiInfractions } from "../../generated/paths-internal";
import { PostApiUsersRoles, GetApiUsersNames as getNames } from "../../generated/paths-public";

// @ts-ignore
new Vue({
	el: "#app",
	data: {
		csrf: null,
		rolesRoute: null,
		infractionsRoute: null,
		roles: [],
		userId: null,

		names: [],
		input: "",

		image: null as HTMLImageElement,
	},
	methods: {
		manageInfractions: function () {
			this.$refs.manage.visible = true;
		},

		removeInfraction: async function (id: number) {
			const res = await DeleteAdminApiInfractions(id, { RequestVerificationToken: this.csrf });
			if (res.ok) location.reload();
		},

		saveRoles: async function () {
			this.roles = [...document.querySelectorAll("input[type=checkbox][name=roles]:checked")].map((e: HTMLInputElement) =>
				Number.parseInt(e.value),
			);
			const res = await PostApiUsersRoles(
				{
					userId: this.userId,
					roles: this.roles,
				},
				{ RequestVerificationToken: this.csrf },
			);

			if (res.ok) {
				location.reload();
			}
		},

		getNames: async function () {
			if (this.input.length < 3) {
				this.names = [];
			} else {
				const res = await getNames(this.input);
				if (res.ok) {
					const data = res.data;
					if (Array.isArray(data)) {
						this.names = data;
					} else {
						console.warn(data);
					}
				}
			}
		},

		showImage: function (e: MouseEvent) {
			if (!this.image) {
				this.image = document.createElement("img");
				this.image.src = (e.target as HTMLAnchorElement).href;
				this.image.height = 200;
				this.image.style.position = "absolute";
				this.image.style.pointerEvents = "none";

				document.body.append(this.image);
			}
			this.image.style.display = "block";
		},

		updateImage: function (e: MouseEvent) {
			this.image.style.left = `${e.clientX}px`;
			this.image.style.top = `${e.clientY}px`;
		},

		hideImage: function () {
			this.image.style.display = "none";
		},
	},
	mounted() {
		const dataElement: HTMLElement = this.$refs.dataElement;
		this.csrf = dataElement.dataset.csrf;

		this.userId = Number(document.getElementById("id").innerText);
	},
});
