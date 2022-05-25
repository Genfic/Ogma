new Vue({
	el: "#blogpost-app",
	methods: {
		report: function () {
			this.$refs.reportModal.visible = true;
		},
	},
});