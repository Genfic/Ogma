import {
	PostApiInviteCodesNoLimit as createUnlimitedCodes,
	DeleteApiInviteCodes as deleteCode,
	GetApiInviteCodesPaginated as getPaginatedCodes,
} from "@g/paths-public";
import type { InviteCodeDto } from "@g/types-public";
import { log } from "@h/logger";
import { long } from "@h/tinytime-templates";

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
			const data = res.data;

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
			const data = res.data;

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
		date: (dt: string) => long.render(new Date(dt)),
	},

	async mounted() {
		// Grab the XCSRF token
		this.xcsrf = (document.querySelector("[name=__RequestVerificationToken]") as HTMLInputElement).value;
		// Grab the initial set of namespaces
		await this.getCodes();
	},
});
