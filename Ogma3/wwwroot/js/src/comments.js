let comments_vue = new Vue({
    el: '#comments-container',
    data: {
        body: null,
        thread: null,
        route: null,
        csrf: null,
        
        comments: [],
        total: 0,
        
        page: 1,
        perPage: null,
        highlight: null,
    },
    methods: {

        // Submit the comment, load comments again, clean textarea
        submit: function (e) {
            e.preventDefault();
            axios.post(this.route,
                {
                    body: this.body,
                    thread: Number(this.thread)
                },{
                    headers: { "RequestVerificationToken" : this.csrf }
                })
                .then(_ => {
                    this.load();
                    this.body = null;
                })
                .catch(console.error)
        },

        // Load comments for the thread
        load: function () {
            const params = {
                thread: this.thread,
                page: this.page
            }
            
            axios.get(this.route, { params: params })
                .then(res => {
                    this.comments = res.data.comments.map(
                        (val, key) => ({val, key})
                    ).reverse();
                    this.total = res.data.totalComments;
                    
                    if (this.highlight) {
                        this.navigateToComment()
                    } else {
                        this.navigateToPage();
                    }
                })
                .catch(console.error)
        },

        // Handle Enter key input
        enter: function(e) {
            if (e.ctrlKey) this.submit(e)
        },

        // Parse date
        date: function (dt) {
            return dayjs(dt).format('DD MMM YYYY, HH:mm');
        },
        
        prevPage: function () {
            this.page = Math.max(1, this.page - 1);
            this.navigateToPage();
            this.load();
        },
        
        nextPage: function () {
            this.page = Math.min(this.page + 1, Math.ceil(this.total / this.perPage));
            this.navigateToPage();
            this.load();
        },
        
        changePage: function (idx) {
            this.page = idx;
            this.navigateToPage();
            this.load();
        },
        
        changeHighlight: function(idx = null, e) {
            e.preventDefault();
            this.highlight = idx ?? this.highlight;
            document
                .getElementById(`comment-${this.highlight}`)
                .scrollIntoView({behavior: "smooth", block: "center", inline: "nearest"});
            history.replaceState(undefined, undefined, `#comment-${idx}`)
        },

        navigateToPage: function () {
            if (this.page > 1) {
                history.replaceState(null, null, `#page-${this.page}`)
            } else {
                history.replaceState(null, null, window.location.href.split('#')[0])
            }
            if (this.highlight) this.highlight = null;
        },
        
        navigateToComment: function () {
            let idx = this.comments.findIndex(e => e.key+1 === this.highlight);
            this.page = Math.floor(idx / this.perPage) + 1;
            this.visibleComments = this.comments.slice((this.page - 1) * this.perPage, this.page * this.perPage);
            Vue.nextTick(function () {
                comments_vue.changeHighlight();
            })
        }
    },

    mounted() {
        this.thread = document.getElementById('thread').dataset.thread;
        this.route = document.getElementById('route').dataset.route;
        this.perPage = document.getElementById('per-page').dataset.count;
        this.csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
        
        let hash = window.location.hash.split('-');
        
        if (hash[0] === '#page' && hash[1]) {
            this.page = Math.max(1, Number(hash[1] ?? 1));
        } else if (hash[0] === '#comment' && hash[1]) {
            this.highlight = Number(hash[1]);
        } else {
            history.replaceState(undefined, undefined, "")
        }
        
        this.load(); 
    }
});