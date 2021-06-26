Vue.component("input-rating", {
	props: {
		label: {
			type: String,
			required: true
		},
		cdn: {
			type: String,
			required: true
		},
		desc: {
			type: String,
			required: false,
			default: null
		},
		ratingsApi: {
			type: String,
			required: true
		},
		value: {
			type: Number,
			default: null
		}
	},
	data: function() {
		return {
			name: this.label.replace(/\s+/g, ""),
			loading: true,
			ratings: []
		};
	},
	methods: {
		checked(id, idx) {
			if (this.value) {
				return id === this.value;
			} else {
				return idx === 0;
			}
		}
	},
	async created() {
		const { data } = await axios.get(this.ratingsApi);
		this.ratings = data;
		this.loading = false;
	},
	template: `
      <div class="o-form-group">
      <label :for="name">{{ label.replace(/([A-Z])/g, " $1") }}</label>
      <p class="desc" v-if="desc">{{ desc }}</p>

      <div v-if="loading">Loading ratings...</div>

      <div v-if="!loading" class="ratings">
        <div v-for="(rating, idx) in ratings" class="rating">

          <input type="radio"
                 :name="name"
                 :id="rating.name.toLowerCase()"
                 :value="rating.id"
                 :checked="checked(rating.id, idx)"
                 class="radio">

          <label class="radio-label active-border" :for="rating.name.toLowerCase()">
            <img :src="cdn+rating.icon" :alt="'Rating: '+rating.name" height="50" width="50">
            <div class="main">
              <strong>{{ rating.name }}</strong>
              <span>{{ rating.description }}</span>
            </div>
          </label>
        </div>
      </div>
      </div>
	`
});