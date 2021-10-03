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
		
		image: null,
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
			if (this.input.length < 3) {
				this.names = [];
			} else {
				const { data } = await axios.get(`${this.rolesRoute}/names`, { params: { name: this.input } });
				this.names = data;
			}
		},
		
		showImage: function(e) {
			if (!this.image) {
				this.image = document.createElement('img');
				this.image.src = e.target.href;
				this.image.style.position = 'absolute';
				this.image.style.pointerEvents = 'none';

				document.body.append(this.image);
			}
			this.image.style.display = 'block';
		},
		
		updateImage: function(e) {
			this.image.style.left = `${e.clientX}px`;
			this.image.style.top = `${e.clientY}px`;
		},
		
		hideImage: function() {
			this.image.style.display = 'none';
		}
	},
	mounted() {
		this.csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
		this.rolesRoute = document.getElementById('rolesRoute').dataset.route;
		this.infractionsRoute = document.getElementById('infractionsRoute').dataset.route;
		this.userId = Number(document.getElementById('id').innerText);
	}
});