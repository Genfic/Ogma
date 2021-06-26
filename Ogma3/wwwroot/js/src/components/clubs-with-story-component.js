Vue.component("clubs-with-story", {
	props: {
		clubsRoute: {
			type: String,
			required: true
		},
		cdn: {
			type: String,
			required: true
		},
		storyId: {
			type: Number,
			required: true
		}
	},
	data: function() {
		return {
			clubs: [],
			visible: false
		};
	},
	methods: {
		hide: function() {
			this.visible = false;
		}
	},
	async mounted() {
		const { data } = await axios.get(`${this.clubsRoute}/story/${this.storyId}`);
		this.clubs = data;
	},
	template: `
      <div class="club-folder-selector my-modal" v-if="visible" @click.self="hide" v-cloak>
      <div class="content">

        <div class="header">
          <span>Featured in</span>
        </div>

        <div v-if="clubs.length > 0" class="clubs">
          <a :href="'/club/'+c.name.toLowerCase().replace(' ', '-')+'-'+c.id"
             target="_blank"
             class="club"
             tabindex="0"
             v-for="c in clubs">
            <img :src="cdn + (c.icon ?? 'ph-250.png')" :alt="c.name" width="24" height="24">
            <span>{{ c.name }}</span>
          </a>
        </div>

        <div v-else>
          This story hasn't been added to any clubs yet.
        </div>

      </div>
      </div>
	`
});