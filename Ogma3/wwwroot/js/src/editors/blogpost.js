new Vue({
	el: '#blogpost',
	data: {}, 
	methods: {
		onSubmit(e) {
			if (!(this.$refs.title.validate && this.$refs.body.validate && this.$refs.tags.validate)) {
				e.preventDefault();
				return false;
			}
		}
	}
});
