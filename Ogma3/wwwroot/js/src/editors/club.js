new Vue({
    el: '#club',
    methods: {
        onSubmit(e) {
            if (!(
                this.$refs.title.validate
                && this.$refs.desc.validate
                && this.$refs.hook.validate
                && this.$refs.cover.validate
            )) {
                e.preventDefault();
                return false;
            }
        },
    },
}); 
