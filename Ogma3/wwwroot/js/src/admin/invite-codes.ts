import { log } from "../../src-helpers/logger";
import {
	DeleteApiInviteCodes as deleteCode,
	GetApiInviteCodesPaginated as getPaginatedCodes,
	PostApiInviteCodesNoLimit as createUnlimitedCodes,
} from "../../generated/paths-public";
import type { InviteCodeDto } from "../../generated/types-public";
import { format } from "date-fns";

// @ts-ignore
new Vue({
	el: "#app",
	data: {
		codes: [] as InviteCodeDto[],
		xcsrf: null as string | null,
		page: 1,
		perPage: 50,
		loading: true,
		completed: false,
		newCode: null as number | null,
	},
	methods: {
		createCode: async function () {
			const res = await createUnlimitedCodes({
				RequestVerificationToken: this.xcsrf,
			});
			const data = await res.json();

			this.newCode = data.id;
			setTimeout(() => {
				this.newCode = null;
			}, 5000);

			this.codes.unshift(data);
		},

		// Gets page of codes
		getCodes: async function () {
			if (this.completed) return;

			this.loading = true;

			const res = await getPaginatedCodes(this.page, this.perPage);
			const data = await res.json();

			if (data.length <= 0) {
				this.completed = true;
				return;
			}

			this.codes = [...this.codes, ...data];
			this.loading = false;
		},

		loadMore: async function () {
			this.page++;
			await this.getCodes();
		},

		// Deletes a selected namespace
		deleteCode: async function (t: InviteCodeDto) {
			if (confirm("Delete permanently?")) {
				const res = await deleteCode(t.id, {
					RequestVerificationToken: this.xcsrf,
				});

				if (!res.ok) return;

				this.codes = this.codes.filter((i: InviteCodeDto) => i.id !== t.id);
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
		date: (dt: string) => format(dt, "dd MMM yyyy, hh:mm"),
	},

	async mounted() {
		// Grab the XCSRF token
		this.xcsrf = (document.querySelector("[name=__RequestVerificationToken]") as HTMLInputElement).value;
		// Grab the initial set of namespaces
		await this.getCodes();
	},
});
