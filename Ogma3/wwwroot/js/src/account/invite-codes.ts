import { log } from "../../src-helpers/logger";
import dayjs from "dayjs";

// @ts-ignore
new Vue({
	el: "#app",
	data: {
		codes: [],
		route: null,
		xcsrf: null
	},
	methods: {

		createCode: async function () {
			const res = await fetch(this.route, { 
				method: 'POST', 
				headers: { RequestVerificationToken: this.xcsrf } 
			}).catch(e => alert(e.response.data));
			
			if (res)
				this.codes.push(await res.json());
		},

		// Gets all existing namespaces
		getCodes: async function () {
			const res = await fetch(this.route);
			
			if (res)
				this.codes = await res.json();
		},

		copyCode: function (t) {
			navigator.clipboard.writeText(t.code).then(
				() => alert("Copied"),
				(e) => {
					alert("Could not copy");
					log.error(e);
				}
			);
		},

		// Parse date
		date: function (dt) {
			return dayjs(dt).format("DD MMM YYYY, HH:mm");
		}
	},

	async mounted() {
		// Grab the route from route helper
		this.route = document.getElementById("route").dataset.route;
		// Grab the XCSRF token
		this.xcsrf = (document.querySelector("[name=__RequestVerificationToken]") as HTMLInputElement).value;
		// Grab the initial set of namespaces
		await this.getCodes();
	}
});