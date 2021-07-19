new Vue({
	el: '#club-bar',
	methods: {
		report: function () {
			this.$refs.reportModal.visible = true;
		}
	}
});