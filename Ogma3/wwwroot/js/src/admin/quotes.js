let quotes_vue = new Vue({
    el: "#app",
    data: {
        form: {
            body: null,
            author: null
        },
        err: [],
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
                    this.quotes = response.data
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
                .catch(console.error)
        }
    },

    computed: {
        filtered() {
            return this.quotes.filter(item => {
                return item.body.toLowerCase().includes(this.search.toLowerCase())
                    || item.author.toLowerCase().includes(this.search.toLowerCase())
            })
        }
    },

    mounted() {
        // Grab the route from route helper
        this.route = document.getElementById('route').dataset.route;
        // Grab the initial set of namespaces
        this.getQuotes();
    }
});