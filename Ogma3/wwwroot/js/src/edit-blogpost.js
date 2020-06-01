new Vue({
    el: '#blogpost',
    data: {}, 
    methods: {
        onSubmit(e) {
            if (!(this.$refs.title.validate && this.$refs.body.validate)) {
                e.preventDefault()
                return false;
            }
        }
    }
});
