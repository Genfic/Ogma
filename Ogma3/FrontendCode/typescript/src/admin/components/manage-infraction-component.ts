import { PostAdminApiInfractions } from "@g/paths-internal";
import type { InfractionType } from "@g/types-internal";
import { log } from "@h/logger";

// @ts-ignore
Vue.component("manage-infraction", {
	props: {
		csrf: {
			type: String,
			required: true,
		},
		userId: {
			type: Number,
			required: true,
		},
		types: {
			type: Array,
			required: true,
		},
	},
	data: () => ({
		type: null as InfractionType,
		date: null as Date,
		reason: null as string,
		visible: false,
	}),
	methods: {
		hide: function () {
			this.type = this.date = this.reason = null;
			this.visible = false;
		},

		create: async function () {
			log.log("submit");
			const res = await PostAdminApiInfractions(
				{
					userId: this.userId,
					reason: this.reason,
					endDate: this.date,
					type: this.type,
				},
				{
					RequestVerificationToken: this.csrf,
				},
			);
			if (!res.ok) return;

			location.reload();
		},
	},
	template: `
		<div class="my-modal" v-if="visible" @click.self="hide">
			<div class="content">
				<strong>Create infraction</strong>
				<hr>

			  	<form class="form" v-on:submit.prevent="create">
				  <div class="o-form-group">
				    <label for="type">Infraction type</label>
				    <select id="type" v-model="type">
				      <option v-for="t in this.types"
				              :value="t"
				              :key="t">
				        {{t}}
				      </option>
				    </select>
				  </div>

				  <div class="o-form-group">
				    <label for="time">Expiration date</label>
				    <input type="date" id="time" v-model="date">
				  </div>

				  <div class="o-form-group">
				    <label for="reason">Reason</label>
				    <textarea id="reason" v-model="reason"></textarea>
				  </div>

				  <div class="o-form-group">
                    <input type="submit" value="Create">
				  </div>
			    </form>

			</div>
		</div>
	`,
});
