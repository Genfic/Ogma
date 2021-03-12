Vue.component('input-toggle', {
	props: {
		label: {
			type: String,
			required: true
		},
		desc: {
			type: String,
			default: null
		},
		value: {
			type: Boolean,
			default: false
		}
	},
	data: function () {
		return {
			name: this.label.replace(/\s+/g, ''),
			checked: this.value,
		};
	},
	methods: {
		update: function (value) {
			this.$emit('input', value);
		}
	},
	watch: {
		value() {
			this.checked = this.value;
		}
	},
	template: `
        <div class="o-form-group keep-size">
            <label>{{label.replace( /([A-Z])/g, " $1" )}}</label>
            <p class="desc" v-if="desc">{{desc}}</p>

            <div class="toggle-input">
                <input type="checkbox"
                     :name="name"
                     :id="name"
                     @change="update($event.target.checked)"
                     v-model="checked"
                     value="true">
              
                <label :for="name">
                    <span class="toggle">
                        <span class="dot"></span>
                    </span>
                  
                    {{checked ? 'On' : 'Off'}}
                </label>
              
            </div>
        </div>
    `
});