let comments_vue = new Vue({
    el: '#comments-container',
    data: {
        body: null,
        thread: null,
        route: null,
        csrf: null,
        
        comments: [],
        total: 0,
        
        page: null,
        perPage: null,
        
        highlight: null,
    },
    methods: {

        // Submit the comment, load comments again, clean textarea
        submit: function (e) {
            e.preventDefault();
            
            let data = {
                body: this.body,
                thread: Number(this.thread)
            };
            
            axios.post(this.route, data,{
                    headers: { "RequestVerificationToken" : this.csrf }
                })
                .then(_ => {
                    this.highlight = this.total + 1;
                    this.page = 1;
                    this.load();
                    this.body = null;
                })
                .catch(console.error)
        },

        // Load comments for the thread
        load: function () {
            const params = {
                thread: this.thread,
                page: this.page,
                highlight: this.highlight
            }
                        
            axios.get(this.route, { params: params })
                .then(res => {
                    this.total = res.data.total;
                    this.page = res.data.page ?? this.page;
                    
                    this.comments = res.data.elements.map(
                        (val, key) => ({val, key: (res.data.total - (this.page * this.perPage)) + (this.perPage - (key + 1))})
                    );
                    
                    if (this.highlight) {
                        Vue.nextTick(_ => comments_vue.changeHighlight())
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
        
        // Navigate to the previous page
        prevPage: function () {
            let page = Math.max(1, this.page - 1);
            this.changePage(page);
        },
        
        // Navigate to the next page
        nextPage: function () {
            let page = Math.min(this.page + 1, Math.ceil(this.total / this.perPage));
            this.changePage(page);
        },
        
        // Navigate to the selected page
        changePage: function (idx) {
            this.page = idx;
            this.navigateToPage();
            this.load();
        },
        
        // Highlights the selected comment and scrolls it into view
        changeHighlight: function(idx = null, e = null) {
            if (e) e.preventDefault();
            this.highlight = idx ?? this.highlight;
            document
                .getElementById(`comment-${this.highlight}`)
                .scrollIntoView({behavior: "smooth", block: "center", inline: "nearest"});
            history.replaceState(undefined, undefined, `#comment-${this.highlight}`)
        },

        // Navigates to `this.page` page
        navigateToPage: function () {
            if (this.page > 1) {
                history.replaceState(null, null, `#page-${this.page}`)
            } else {
                history.replaceState(null, null, window.location.href.split('#')[0])
            }
            if (this.highlight) this.highlight = null;
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
            this.page = 1;
            this.highlight = Number(hash[1]);
        } else {
            this.page = 1;
            history.replaceState(undefined, undefined, "")
        }
        
        this.load(); 
    }
});