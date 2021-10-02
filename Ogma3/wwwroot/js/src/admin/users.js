new Vue({
	el: '#app',
	data: {
		csrf: null,
		rolesRoute: null,
		infractionsRoute: null,
		roles: [],
		userId: null,
		
		names: [],
		input: '',
	},
	methods: {
		manageBan: function () {
			this.$refs.manageBan.visible = true;
		},
		
		manageMute: function () {
			this.$refs.manageMute.visible = true;
		},
		
		saveRoles: async function () {
			this.roles = [...document.querySelectorAll('input[type=checkbox][name=roles]:checked')].map(e => Number(e.value));
			await axios.post(`${this.rolesRoute}/roles`, {
				UserId: this.userId,
				Roles: this.roles
			},{
				headers: { "RequestVerificationToken" : this.csrf }
			});
			location.reload();
		},
		
		getNames: async function() {
			if (this.input.length < 3) this.names = [];
			const { data } = await axios.get(`${this.rolesRoute}/names`, { params: { name: this.input }})
				.catch(e => {
					if(e.response.status !== 422) throw e;
				});
			this.names = data;
		}
	},
	mounted() {
		this.csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
		this.rolesRoute = document.getElementById('rolesRoute').dataset.route;
		this.infractionsRoute = document.getElementById('infractionsRoute').dataset.route;
		this.userId = Number(document.getElementById('id').innerText);
	}
});