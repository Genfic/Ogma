let story_vue = new Vue({
    el: '#story-details',
    data: {
        score: 0,
        pool: null,
        route: null,
        didVote: false
    },
    methods: {
        vote: function() {
            axios.post(this.route, {
                votePool: Number(this.pool)
            })
            .then(res => {
                this.score = res.data.count;
                this.didVote = res.data.didVote;
            })
            .catch(console.error)
        },
    },
    mounted() {
        this.pool = document.getElementById('pool-id').dataset.pool;
        this.route = document.getElementById('route').dataset.route;
        axios.get(this.route + '/' + this.pool)
            .then(res => {
                console.log(res.data);
                this.score = res.data.count;
                this.didVote = res.data.didVote;
            })
            .catch(console.error);
    }
});