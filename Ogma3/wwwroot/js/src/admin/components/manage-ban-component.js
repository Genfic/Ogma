Vue.component('manage-ban', {
	props: {
		actionName: {
			type: String,
			required: true  
		},
		banDate: {
			type: String,
			required: false
		},
		duration: {
			type: Number,
			required: true
		},
		route: {
			type: String,
			required: true
		},
		userId: {
			type: Number,
			required: true
		}
	},
	data: function () {
		return {            
			newDuration: 0,
			days: 1,
			csrf: null,
			visible: false
		};
	},
	methods: {
		hide: function () {
			this.visible = false;
		},
		ban: function () {
			axios.post(`${this.route}/${this.actionName.toLowerCase()}`, {
				UserId: this.userId,
				Days: this.days
			},{
				headers: { "RequestVerificationToken" : this.csrf }
			})
				.then(() => location.reload())
				.catch(console.error);
		},
		unban: function () {
			axios.post(`${this.route}/${this.actionName.toLowerCase()}`, {
				UserId: this.userId,
				Days: null
			},{
				headers: { "RequestVerificationToken" : this.csrf }
			})
				.then(() => location.reload())
				.catch(console.error);
		}
	},
	template: `
      <div class="my-modal" v-if="visible" @click.self="hide">
          <div class="content">
            <strong>Manage {{actionName.toLowerCase()}}</strong>
            <hr>
            
            <template v-if="banDate">
              <time :datetime="banDate">Lasts until {{banDate}}</time>
              <br>
              <span><strong>{{duration}}</strong> days left</span>
              <hr>
              <button @click="unban">Un{{actionName.toLowerCase()}}</button>
            </template>
            
            <template v-else>
              {{actionName}} user for <input type="number" min="1" v-model="days"> days
              <br>
              <button @click="ban">Save</button>
            </template>
            
          </div>
        </div>
    `,
	mounted() {
		this.csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
	}
});