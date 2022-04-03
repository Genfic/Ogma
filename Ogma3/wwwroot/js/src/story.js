new Vue({
    el: '#story-app',
    data: {},
    methods: {
        addToFolder: function () {
            this.$refs.folderSelect.visible = true;
        },
        showClubs: function () {
            this.$refs.featuredIn.visible = true;
        },
        report: function () {
            this.$refs.reportModal.visible = true;
        }
    }
});