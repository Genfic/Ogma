new Vue({
	el: '#app',
	data: {
		csrf: null,
		rolesRoute: null,
		infractionsRoute: null,
		roles: [],
		userId: null,
	},
	methods: {
		manageBan: function () {
			this.$refs.manageBan.visible = true;
		},
		manageMute: function () {
			this.$refs.manageMute.visible = true;
		},
		saveRoles: async function () {
			this.roles = [...document.querySelectorAll('input[type=checkbox]:checked')].map(e => Number(e.value));
			await axios.post(`${this.rolesRoute}/roles`, {
				UserId: this.userId,
				Roles: this.roles
			},{
				headers: { "RequestVerificationToken" : this.csrf }
			});
			location.reload();
		}
	},
	mounted() {
		this.csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
		this.rolesRoute = document.getElementById('rolesRoute').dataset.route;
		this.infractionsRoute = document.getElementById('infractionsRoute').dataset.route;
		this.userId = Number(document.getElementById('id').innerText);
	}
});