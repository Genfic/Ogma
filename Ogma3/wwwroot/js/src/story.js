new Vue({
	el: "#story-app",
	data: {},
	methods: {
		addToFolder: function () {
			this.$refs.folderSelect.visible = true;
		},
		report: function () {
			this.$refs.reportModal.visible = true;
		},
	},
});