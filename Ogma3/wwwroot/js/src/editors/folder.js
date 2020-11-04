new Vue({
    el: '#folder',
    methods: {
        onSubmit(e) {
            if (!(this.$refs.name.validate && this.$refs.desc.validate)) {
                e.preventDefault()
                return false;
            }
        }
    }
});
