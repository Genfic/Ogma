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
			return dayjs(time).format('DD MMMM YYYY, HH:mm');
		}
	},
	mounted() {
		this.route = document.querySelector('[data-route]').dataset.route;
		this.fetch();
		this.csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
	}
}); 