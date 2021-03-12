import dayjs from 'https://cdn.skypack.dev/-/dayjs@v1.10.4-MoS2QVkxh1TZYPgJA5zq/dist=es2020,mode=imports/optimized/dayjs.js';

new Vue({
	el: '#notifications',
	data: {
		notifications: [],
		route: null,
		csrf: null,
	},
	methods: {
		fetch: function () {
			axios.get(this.route)
				.then(data => {
					this.notifications = data.data;
					document.querySelector('o-notifications').load();
				})
				.catch(console.error);
		},
        
		deleteNotif: function (id) {
			axios.delete(`${this.route}/${id}`,
				{
					headers: { "RequestVerificationToken" : this.csrf }
				})
				.then(() => {
					this.fetch();
				})
				.catch(console.error);
		},
        
		parseTime: function (time) {
			let day = dayjs(time);
			return day.format('DD MMMM YYYY, HH:mm');
		}
	},
	mounted() {
		this.route = document.querySelector('[data-route]').dataset.route;
		this.fetch();
		this.csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
	}
}); 