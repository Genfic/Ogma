new Vue({
	el: "#chapter-app",
	methods: {
		report: function () {
			this.$refs.reportModal.visible = true;
		},
	},
});