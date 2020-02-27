let story_vue = new Vue({
    el: '#story-details',
    data: {
        score: 0,
        pool: null,
        route: null
    },
    methods: {
        vote: function() {
            axios.post(this.route, {
                votePool: Number(this.pool)
            })
            .then(_ => {
            })
            .catch(console.error)
        }
    },
    mounted() {
        this.pool = document.getElementById('pool-id').dataset.pool;
        this.route = document.getElementById('route').dataset.route;
    }
});