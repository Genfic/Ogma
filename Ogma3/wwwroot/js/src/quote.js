new Vue({
    el: '#quote',
    data: {
        quote: null,
        author: null,
        route: null,
        loading: true
    },
    methods: {
        fetch: function () {
            this.loading = true;
            axios.get(this.route)
                .then(res => {
                    this.quote = res.data.body;
                    this.author = res.data.author;
                })
                .catch(console.error)
                .finally(() => this.loading = false)
        }
    },
    mounted() {
        this.route = document.getElementById('quote-route').dataset.route;
        this.fetch()
    }
})