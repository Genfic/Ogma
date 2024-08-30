import { PostApiReports as report } from "../../generated/paths-public";

Vue.component("vue-report-modal", {
	props: {
		csrf: {
			type: String,
			required: true,
		},
		itemType: {
			type: String,
			required: true,
		},
		itemId: {
			type: Number,
			required: true,
		},
	},
	data: function () {
		return {
			visible: false,
			reason: "",
			message: null,
			btnClass: "",
			mutId: this.itemId,
		};
	},
	methods: {
		hide: function () {
			this.reason = "";
			this.visible = false;
			this.$refs.text.clear();
		},
		send: async function () {
			if (!this.$refs.text.validate) return;

			const res = await report(
				{
					itemId: this.mutId,
					reason: this.reason,
					itemType: this.itemType,
				},
				{ RequestVerificationToken: this.csrf },
			);

			if (res.ok) {
				this.message = "Report delivered!";
				this.btnClass = "green";
			} else {
				this.message = "An error has occurred.";
				this.btnClass = "red";
			}
		},
	},

	template: `
		<div class="report-modal my-modal" v-if="visible" @click.self="hide" v-cloak>
		<div class="content">

			<div class="header">
				<span>Report content</span>
			</div>

			<form class="form">
				<textarea-counter ref="text"
								  label="Reason"
								  desc="Why do you want to report this content?"
								  validate-msg="The {0} must be between {2} and {1} characters"
								  :min="30" :max="500"
								  v-model="reason">
				</textarea-counter>
			</form>

			<br>

			<button class="btn" :class="btnClass" v-on:click="send">
				{{ message ?? 'Send report' }}
			</button>

		</div>
		</div>
	`,
});
