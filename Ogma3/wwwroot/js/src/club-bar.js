new Vue({
	el: '#club-bar',
	data: {
		route: null,
		id: null,
		xcsrf: null,
		joined: null,
	},
	methods: {
		joinOrLeave: async function(){
			this.joined ? await this.leave() : await this.join();
		},
		join: async function () {
			const {data} = await axios.post(this.route,
				{ ClubId: this.id }
			);
			this.joined = data;
		},
		leave: async function() {
			const {data} = await axios.delete(this.route,
				{ data: { ClubId: this.id } }
			);
			this.joined = data;
		},
		report: function () {
			this.$refs.reportModal.visible = true;
		}
	},
	mounted() {        
		this.route = document.getElementById('data-route').dataset.route;
		this.id = Number(document.getElementById('data-id').dataset.id);
		this.joined = document.getElementById('data-joined').dataset.joined.toLowerCase() === 'true';
	},
	beforeCreate() {
		document.getElementById('join-btn').classList.remove('join', 'leave');
	}
});