let story_vue = new Vue({
    el: '#story-app',
    data: {
        votesRoute: null,
        shelvesRoute: null,
        readsRoute: null,
        csrf: null,
        
        storyId: null,
        
        shelves: [],
        showShelves: false,
        
        read: []
    },
    methods: {
        
        // Gets all existing shelves
        getShelves: function() {
            axios.get(this.shelvesRoute + '/user/' + this.storyId)
                .then(response => {
                    this.shelves = response.data || [];
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
                .then(_ => {
                    this.getShelves();
                })
                .catch(console.error)
        },
        
        markRead: function (chapter) {
            axios.post(this.readsRoute, 
                { 
                    story: this.storyId, 
                    chapter: chapter 
                },
                {
                    headers: { "RequestVerificationToken" : this.csrf }
                })
                .then(res => {
                    this.read = res.data.read || [];
                })
                .catch(console.error);
        },
        
        addToFolder: function () {
            this.$refs.folderSelect.visible = true;
        },
        showClubs: function () {
            this.$refs.featuredIn.visible = true;
        },
        report: function () {
            this.$refs.reportModal.visible = true;
        }
    },
    mounted() {
        // Get story ID
        this.storyId = Number(document.getElementById('story-id').dataset.id);
        // Get routes
        this.shelvesRoute = document.getElementById('shelves-route').dataset.route;
        this.readsRoute = document.getElementById('reads-route').dataset.route;
        // Get CSRF token
        this.csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
        // get initial reads
        axios.get(this.readsRoute + '/' + this.storyId)
            .then(res => {
                this.read = res.data.read || [];
            })
            .catch(console.error);
        // Get shelves
        this.getShelves()
    }
});