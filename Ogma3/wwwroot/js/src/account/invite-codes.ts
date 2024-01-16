import dayjs from "dayjs";
import { log } from "../../src-helpers/logger";
import {
	InviteCodes_GetInviteCodes as getCodes,
	InviteCodes_PostInviteCode as createCode,
} from "../../generated/paths-public";
import { type InviteCodeDto } from "../../generated/types-public";

// @ts-ignore
new Vue({
	el: "#app",
	data: {
		codes: [] as InviteCodeDto[],
		route: null as string | null,
		xcsrf: null as string | null,
	},
	methods: {
		createCode: async function () {
			const res = await createCode({ RequestVerificationToken: this.xcsrf });

			if (res) {
				this.codes.push(await res.json());
			}
		},

		// Gets all existing namespaces
		getCodes: async function () {
			const res = await getCodes();

			if (res) {
				this.codes = await res.json();
			}
		},

		copyCode: (t: InviteCodeDto) => {
			navigator.clipboard.writeText(t.code).then(
				() => alert("Copied"),
				(e) => {
					alert("Could not copy");
					log.error(e);
				},
			);
		},

		// Parse date
		date: (dt: string) => dayjs(dt).format("DD MMM YYYY, HH:mm"),
	},

	async mounted() {
		// Grab the route from route helper
		this.route = document.getElementById("route").dataset.route;
		// Grab the XCSRF token
		this.xcsrf = (
			document.querySelector(
				"[name=__RequestVerificationToken]",
			) as HTMLInputElement
		).value;
		// Grab the initial set of namespaces
		await this.getCodes();
	},
});
