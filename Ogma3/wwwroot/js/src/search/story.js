const search_vue = new Vue({
    el: '#search',
    data: {
        els: {
            query: null,
            rating: null,
            sort: null,
            tags: null,
        },
        dis: {
            query: false,
            rating: false,
            sort: false,
            tags: false,
        }
    },
    methods: {
        submit: function (e) {
            this.els.tags = this.$refs.tags.selected.length > 0;
            for(let[key, val] of Object.entries(this.els)) {
                this.dis[key] = !val;
            }
            this.dis.tags = this.$refs.tags.checkDisabled();
            setTimeout(_ => e.target.submit())
        }
    },
    mounted() {
        console.log(document.getElementById('query'));
        this.els = {
            query: document.getElementById('query').value,
            rating: document.getElementById('rating').value,
            sort: document.getElementById('sort').value,
            tags: [],
        }
    }
});