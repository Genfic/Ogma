Vue.component('club-role-slider', {
	props: {
		rolesStr: {
			type: String,
			required: true
		},
		label: {
			type: String,
			required: true
		},
		value: {
			type: Number,
		},
		desc: {
			type: String,
		},
	},
	data: function() {
		return {
			roles: JSON.parse(this.rolesStr).reverse(),
			name: this.label.replace(/\s+/g, ''),
			selected: this.value ?? 0,
		}; 
	},
	methods: {}, 
	template: `
      <div class="o-form-group club-role-slider">
      <label>{{label.replace( /([A-Z])/g, " $1" )}}</label>
      <p class="desc" v-if="desc">{{desc}}</p>
      
      <div class="selector active-border">
        <template v-for="(r, k) in roles">
            <input type="radio" 
                   :id="'sel-' + r" 
                   :name="name" 
                   :value='roles.length - 1 - k'
                   v-model="selected">
            <label :for="'sel-' + r" tabindex="0">{{ r }}</label>
        </template>
      </div>
      
      </div>
    `
});