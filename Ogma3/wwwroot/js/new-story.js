new Vue({
    el: '#story',
    data: {
        // Form counters
        title: {
            max: 0,
            min: 0,
            current: '',
        },
        hook: {
            max: 0,
            min: 0,
            current: '',
        },
        desc: {
            max: 0,
            min: 0,
            current: '',
        },
        
        // Tag search
        options: [],
        selected: [],
        search: '',
        
        // API routing
        route: null
    },  
    methods: {
        addUnique(x) {
            if (!this.selected.includes(x))
                this.selected.push(x);
        },
        remove(x) {
            this.selected = this.selected.filter(e => e.id !== x.id);
        }
    },
    computed: {
        titleCount() {
            return `${this.title.current.length}/${this.title.max}`;
        },
        hookCount() {
            return `${this.hook.current.length}/${this.hook.max}`;
        },
        descCount() {
            return `${this.desc.current.length}/${this.desc.max}`;
        },
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

        let title = document.getElementById('Input_Title');
        this.title.max = title.dataset.valLengthMax;
        this.title.min = title.dataset.valLengthMin;

        let hook = document.getElementById('Input_Hook');
        this.hook.max = hook.dataset.valLengthMax;
        this.hook.min = hook.dataset.valLengthMin;

        let desc = document.getElementById('Input_Description');
        this.desc.max = desc.dataset.valLengthMax;
        this.desc.min = desc.dataset.valLengthMin;

        axios.get(this.route)
            .then(res => this.options = res.data)
            .catch(console.error)
    }
});
