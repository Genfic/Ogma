new Vue({
    el: '#chapter',
    data: {},
    methods: {
        onSubmit(e) {
            if (!(
                this.$refs.title.validate
                && this.$refs.body.validate
                && this.$refs.startNotes.validate
                && this.$refs.endNotes.validate
            )) {
                e.preventDefault();
                return false;
            }
        }
    }
});
