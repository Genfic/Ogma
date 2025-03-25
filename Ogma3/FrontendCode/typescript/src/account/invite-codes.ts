import { PostApiInviteCodes as createCode, GetApiInviteCodes as getCodes } from "../../generated/paths-public";
import type { InviteCodeDto } from "../../generated/types-public";
import { log } from "../../src-helpers/logger";
import { long } from "../../src-helpers/tinytime-templates";

// @ts-ignore
new Vue({
	el: "#app",
	data: {
		codes: [] as InviteCodeDto[],
		xcsrf: null as string | null,
	},
	methods: {
		createCode: async function () {
			const res = await createCode({ RequestVerificationToken: this.xcsrf });

			if (res.ok) {
				this.codes.push(res.data);
			}
		},

		// Gets all existing namespaces
		getCodes: async function () {
			const res = await getCodes();

			if (res.ok) {
				this.codes = res.data;
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
		date: (dt: string) => long.render(new Date(dt)),
	},

	async mounted() {
		// Grab the XCSRF token
		this.xcsrf = (document.querySelector("[name=__RequestVerificationToken]") as HTMLInputElement).value;
		// Grab the initial set of namespaces
		await this.getCodes();
	},
});
