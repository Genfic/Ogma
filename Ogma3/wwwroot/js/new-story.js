new Vue({
    el: '#story',
    data: {
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
        route: null
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
