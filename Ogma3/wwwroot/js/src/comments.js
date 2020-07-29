let comments_vue = new Vue({
    el: '#comments-container',
    data: {
        body: null,
        thread: null,
        route: null,
        csrf: null,
        comments: [],
        visibleComments: [],
        page: 1,
        perPage: 10
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
            axios.get(this.route, { params:{ thread:this.thread } })
                .then(res => {
                    this.comments = res.data.map(
                        (val, key) => ({val, key})
                    ).reverse();
                    this.paginate();
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
            this.paginate()
        },
        
        nextPage: function () {
            this.page = Math.min(this.page + 1, this.comments.length / this.perPage);
            this.paginate()
        },
        
        changePage: function (idx) {
            this.page = idx;
            this.paginate()
        },

        paginate: function () {
            this.visibleComments = this.comments.slice((this.page - 1) * this.perPage, this.page * this.perPage);
            window.location.hash = this.page;
        }
    },

    mounted() {
        this.thread = document.getElementById('thread').dataset.thread;
        this.route = document.getElementById('route').dataset.route;
        this.csrf = document.querySelector('input[name=__RequestVerificationToken').value;
        this.page = Math.max(1, Number(window.location.hash.substring(1)));
        this.load();
    } 
});