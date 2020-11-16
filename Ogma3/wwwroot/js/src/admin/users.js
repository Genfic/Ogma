new Vue({
    el: '#app',
    data: {
        
    },
    methods: {
        manageBan: function () {
            this.$refs.manageBan.visible = true;
        },
        manageMute: function () {
            this.$refs.manageMute.visible = true;
        }
    }
})