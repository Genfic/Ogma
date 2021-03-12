new Vue({
	el: '#story',
	methods: {
		onSubmit(e) {
			if (!(
				this.$refs.title.validate 
                && this.$refs.desc.validate 
                && this.$refs.hook.validate
                && this.$refs.cover.validate
                && this.$refs.tags.validate
			)) {
				e.preventDefault();
				return false;
			}
		},
	},
}); 
