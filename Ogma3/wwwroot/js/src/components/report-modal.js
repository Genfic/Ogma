import { log } from "../../src-helpers/logger";

Vue.component("vue-report-modal", {
	props: {
		reportsRoute: {
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
			csrf: null,
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

			try {
				await axios.post(
					`${this.reportsRoute}`,
					{
						itemId: this.mutId,
						reason: this.reason,
						itemType: this.itemType,
					},
					{
						headers: { RequestVerificationToken: this.csrf },
					},
				);
				this.message = "Report delivered!";
				this.btnClass = "green";
			} catch (e) {
				this.message = "An error has occurred.";
				this.btnClass = "red";
				log.error(e);
			}
		},
	},
	mounted() {
		this.csrf = document.querySelector("input[name=__RequestVerificationToken]").value;
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
