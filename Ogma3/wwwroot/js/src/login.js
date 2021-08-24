new Vue({ 
	el: '#app',
	data: {
		name: null,
		avatar: 'key.webp',
		title: null,
		checked: false,        
        
		cdn: null,
		route: null
	},
	methods: {
		checkDetails: async function (e) {
			e.preventDefault();
            
			if (this.name) {
				const {data, status} = await axios.get(`${this.route}?name=${this.name}`);

				if(status === 200) {
					this.avatar = data.avatar;
					this.title = data.title;
					this.checked = true;
				}
			}
		},
		reset: function () {
			this.avatar = null;
			this.title = null;
			this.checked = false;
		}
	},
    
	computed: {
		getAvatar: function() {
			return this.avatar?.includes('picsum') || this.avatar?.includes('gravatar')
				? this.avatar 
				: this.cdn + (this.avatar ?? 'key.webp');
		}
	},
    
	mounted() {
		// Grab the route from route helper
		this.route = document.getElementById('route').dataset.route; 
		this.cdn = document.getElementById('cdn').dataset.cdn;
	}
});

