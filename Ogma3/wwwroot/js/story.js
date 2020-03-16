let story_vue = new Vue({
    el: '#story-app',
    data: {
        votesRoute: null,
        shelvesRoute: null,
        
        score: 0,
        pool: null,
        didVote: false,
        
        storyId: null,
        
        shelves: [],
        showShelves: false
    },
    methods: {
        vote: function() {
            axios.post(this.votesRoute, {
                votePool: Number(this.pool)
            })
            .then(res => {
                this.score = res.data.count;
                this.didVote = res.data.didVote;
            })
            .catch(console.error)
        },
        
        // Gets all existing shelves
        getShelves: function() {
            axios.get(this.shelvesRoute + '/user/', {
                params: {
                    story: this.storyId
                }
            })
                .then(response => {
                    this.shelves = response.data
                })
                .catch(console.error);
        },
        
        // Add to bookshelf
        addToShelf: function (shelf) {
            console.log(shelf, this.storyId);
            axios.post(this.shelvesRoute + "/add/" + shelf + "/" + this.storyId, 
                null,
                {
                    headers: { "RequestVerificationToken" : this.csrf }
                })
                .then(res => {
                    console.log(res.data);
                    this.getShelves();
                })
                .catch(console.error)
        }
    },
    mounted() {
        // Get pool and story IDs
        this.pool = document.getElementById('pool-id').dataset.pool;
        this.storyId = document.getElementById('story-id').dataset.id;
        // Get routes
        this.votesRoute = document.getElementById('votes-route').dataset.route;
        this.shelvesRoute = document.getElementById('shelves-route').dataset.route;
        // Get CSRF token
        this.csrf = document.querySelector('input[name=__RequestVerificationToken').value;
        // Get initial score
        axios.get(this.votesRoute + '/' + this.pool)
            .then(res => {
                this.score = res.data.count;
                this.didVote = res.data.didVote;
            })
            .catch(console.error);
        // Get shelves
        this.getShelves()
    }
});