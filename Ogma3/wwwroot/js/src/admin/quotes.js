let quotes_vue = new Vue({
	el: "#app",
	data: {
		form: {
			body: null,
			author: null
		},
		quotes: [],
		route: null,
		json: null,
		search: '',
	},
	methods: {
        
		// Gets all existing namespaces
		getQuotes: function () {
			axios.get(this.route)
				.then(response => {
					this.quotes = response.data;
				})
				.catch(error => {
					console.log(error);
				});
		},
        
		deleteQuote: function(q) {
			if(confirm("Delete permanently?")) {
				axios.delete(this.route + '/' + q.id)
					.then(res => {
						this.quotes = this.quotes.filter(i => i.id !== res.data.id);
					})
					.catch(console.error); 
			}
		},
        
		editQuote: function() {},
                
		// Upload Json
		fromJson: function() {
			axios.post('/api/quotes/json', JSON.parse(this.json))
				.then(console.log)
				.catch(console.error);
		}
	},
    
	watch: {
		search() {
			for (const q of this.quotes) {
				q.show = this.search 
					? q.body.toLowerCase().includes(this.search.toLowerCase()) || q.author.toLowerCase().includes(this.search.toLowerCase())
					: true;
			}
		}
	},

	mounted() {
		// Grab the route from route helper
		this.route = document.getElementById('route').dataset.route;
		// Grab the initial set of namespaces
		this.getQuotes();
	}
});