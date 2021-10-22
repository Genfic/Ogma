Vue.component('input-blog-tags', {
	props: {
		max: {
			type: Number,
			required: true
		},
		label: {
			type: String,
			required: true
		},
		value: {
			type: String,
			default: ''
		},
		desc: {
			type: String,
			required: false,
			default: null
		},
		validateMsg: {
			type: String,
			default: null
		}
	},
	data: function () {
		log.log(this.value);
		return {
			text: this.value,
			name: this.label.replace(/\s+/g, '')
		};
	},
	computed: {
		countTags: function () {
			return this.text ? this.text.split(',').length : 0;
		},
		validate: function () {
			return this.text.split(',').length <= this.max;
		},
		validationString: function () {
			return this.validateMsg
				.replace('{0}', this.label)
				.replace('{1}', `${this.max}`);
		}
	},
	template: `
        <div class="o-form-group">
            <label :for="name">{{label.replace( /([A-Z])/g, " $1" )}}</label>
            <p class="desc" v-if="desc">{{desc}}</p>
            <input :name="name"
                   :id="name" 
                   type="text" 
                   class="o-form-control active-border" 
                   v-model="text">
            <div class="counter" :class="validate ? '' : 'invalid'">
                <div class="o-progress-bar" :style="{ width: Math.min(100, 100 * (countTags / max)) + '%' }"></div>
                <span>{{countTags}}/{{max.toLocaleString()}}</span>
            </div>
            <span v-if="!validate && validateMsg">{{validationString}}</span>
        </div>
    `
});