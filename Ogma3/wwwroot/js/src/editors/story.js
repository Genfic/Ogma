new Vue({
    el: '#story',
    data: {
        // Tag search
        options: [],
        selected: [],
        search: '',

        // API routing
        route: null,
    }, 
    methods: {
        onSubmit(e) {
            if (!(this.$refs.title.validate && this.$refs.desc.validate && this.$refs.hook.validate)) {
                e.preventDefault()
                return false;
            }
        },
        addUnique(x) {
            if (this.selected.includes(x)) return;
            this.selected.push(x);
            this.options.find(e => e.id === x.id).hidden = true;
        },
        remove(x) {
            this.selected = this.selected.filter(e => e.id !== x.id);
            this.options.find(e => e.id === x.id).hidden = false;
        }
    },
    computed: {
        filtered() {
            return this.options.filter(x => {
                return (
                    x.name.toLowerCase().includes(this.search.toLowerCase())
                    || x.namespace.toLowerCase().includes(this.search.toLowerCase())
                ) && this.search.length > 0
            })
        }
    },
    beforeMount() {
        this.route = document.getElementById('route').dataset.route;
        axios.get(this.route)
            .then(res => this.options = res.data)
            .catch(console.error)
    }
}); 
