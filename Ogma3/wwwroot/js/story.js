let story_vue = new Vue({
    el: '#story-details',
    data: {
        score: 0,
        pool: null,
        route: null,
        icon: 'star_border'
    },
    methods: {
        vote: function() {
            axios.post(this.route, {
                votePool: Number(this.pool)
            })
            .then(res => {
                this.score = res.data;
            })
            .catch(console.error)
        }
    },
    mounted() {
        this.pool = document.getElementById('pool-id').dataset.pool;
        this.route = document.getElementById('route').dataset.route;
        axios.get(this.route + '/' + this.pool)
            .then(res => {
                this.score = res.data;
            })
            .catch(console.error);
        
        axios.get(this.route + '/uservote/' + this.pool)
            .then(res => {
                this.icon = res.data ? 'star' : 'star_border';
            })
            .catch(console.error)
    }
});