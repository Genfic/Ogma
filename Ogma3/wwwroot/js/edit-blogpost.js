new Vue({
    el: '#blogpost',
    data: {
        // Form counters
        title: {
            max: 0,
            min: 0,
            current: '',
        },
        body: {
            max: 0,
            min: 0,
            current: '',
        },
    }, 
    computed: {
        titleCount() {
            return `${this.title.current.length}/${this.title.max}`;
        },
        bodyCount() {
            return `${this.body.current.length}/${this.body.max}`;
        },
    },
    beforeMount() {
        let title = document.getElementById('Input_Title');
        this.title.max = title.dataset.valLengthMax;
        this.title.min = title.dataset.valLengthMin;
        this.title.current = document.getElementById('v-title').dataset.title;

        let body = document.getElementById('Input_Body');
        this.body.max = body.dataset.valLengthMax;
        this.body.min = body.dataset.valLengthMin;
        this.body.current = document.getElementById('v-body').dataset.body;
    }
});
