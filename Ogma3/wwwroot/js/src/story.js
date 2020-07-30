let story_vue = new Vue({
    el: '#story-app',
    data: {
        votesRoute: null,
        shelvesRoute: null,
        readsRoute: null,
        csrf: null,
        
        score: 0,
        pool: null,
        didVote: false,
        
        storyId: null,
        
        shelves: [],
        showShelves: false,
        
        read: []
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
            axios.get(this.shelvesRoute + '/user/' + this.storyId)
                .then(response => {
                    this.shelves = response.data
                })
                .catch(console.error);
        },
        
        // Add to bookshelf
        addToShelf: function (shelf) {
            axios.post(this.shelvesRoute + "/add/" + shelf + "/" + this.storyId, 
                null,
                {
                    headers: { "RequestVerificationToken" : this.csrf }
                })
                .then(res => {
                    this.getShelves();
                })
                .catch(console.error)
        },
        
        markRead: function (chapter) {
            axios.post(this.readsRoute, { story: this.storyId, chapter: chapter })
                .then(res => {
                    this.read = res.data.read;
                })
                .catch(console.error);
        }
    },
    mounted() {
        // Get pool and story IDs
        this.pool = Number(document.getElementById('pool-id').dataset.pool);
        this.storyId = Number(document.getElementById('story-id').dataset.id);
        // Get routes
        this.votesRoute = document.getElementById('votes-route').dataset.route;
        this.shelvesRoute = document.getElementById('shelves-route').dataset.route;
        this.readsRoute = document.getElementById('reads-route').dataset.route;
        // Get CSRF token
        this.csrf = document.querySelector('input[name=__RequestVerificationToken').value;
        // Get initial score
        axios.get(this.votesRoute + '/' + this.pool)
            .then(res => {
                this.score = res.data.count;
                this.didVote = res.data.didVote;
            })
            .catch(console.error);
        // get initial reads
        axios.get(this.readsRoute + '/' + this.storyId)
            .then(res => {
                this.read = res.data.read
            })
            .catch(console.error);
        // Get shelves
        this.getShelves()
    }
});