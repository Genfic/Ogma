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
        let title = document.getElementById('Blogpost_Title');
        this.title.max = title.dataset.valLengthMax;
        this.title.min = title.dataset.valLengthMin;

        let body = document.getElementById('Blogpost_Body');
        this.body.max = body.dataset.valLengthMax;
        this.body.min = body.dataset.valLengthMin;
    }
});
