import { GetApiSignin as getSignInData } from "../generated/paths-public";

// @ts-ignore
new Vue({
	el: "#app",
	data: {
		name: null as string | null,
		avatar: "https://picsum.photos/200",
		title: null as string | null,
		checked: false,
	},
	methods: {
		checkDetails: async function (e: Event) {
			e.preventDefault();

			if (this.name) {
				const res = await getSignInData(this.name)

				if (res.ok) {
					const data = await res.json();
					this.avatar = data.avatar;
					this.title = data.title;
					this.checked = true;
				}
			}
		},
		reset: function () {
			this.avatar = null;
			this.title = null;
			this.checked = false;
		},
	},
});
