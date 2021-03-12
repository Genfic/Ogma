import dayjs from 'https://cdn.skypack.dev/-/dayjs@v1.10.4-MoS2QVkxh1TZYPgJA5zq/dist=es2020,mode=imports/optimized/dayjs.js';

new Vue({
	el: "#app",
	data: {
		codes: [],
		route: null,
		xcsrf: null,
	},
	methods: {

		createCode: function() {
			axios.post(this.route, null, { headers: { RequestVerificationToken: this.xcsrf } })
				.then(response => {
					this.codes.push(response.data);
				})
				.catch(error => {
					console.log(error);
				});
		},

		// Gets all existing namespaces
		getCodes: function () {
			axios.get(this.route)
				.then(response => {
					this.codes = response.data;
				})
				.catch(error => {
					console.log(error);
				});
		},
        
		copyCode: function(t) {
			navigator.clipboard.writeText(t.code).then(
				( ) => alert("Copied"), 
				(e) => {
					alert("Could not copy");
					console.error(e);
				}
			);
		},

		// Parse date
		date: function (dt) {
			return dayjs(dt).format('DD MMM YYYY, HH:mm');
		}
	}, 

	mounted() {
		// Grab the route from route helper
		this.route = document.getElementById('route').dataset.route;
		// Grab the XCSRF token
		this.xcsrf = document.querySelector('[name=__RequestVerificationToken]').value; 
		// Grab the initial set of namespaces
		this.getCodes();
	}
});