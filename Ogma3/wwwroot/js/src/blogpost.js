new Vue({
	el: '#blogpost-app',
	data: {
		ticking: false,
      
		windowHeight: 0,
		containerHeight: 0,
		progress: 0,
	},
	methods: {
		report: function () {
			this.$refs.reportModal.visible = true;
		},
		handleScroll: function () {
			let elBottom = this.$el.getBoundingClientRect().bottom;
			let perc = elBottom - this.windowHeight;
         
			this.progress = 1 - perc.normalize(0, Math.max(this.containerHeight - this.windowHeight, 0)).clamp();
		}
	},
	mounted() {
		this.windowHeight = window.innerHeight;
		this.containerHeight = this.$el.offsetTop + this.$el.offsetHeight;
      
		document.addEventListener('scroll', () => {
			if (!this.ticking) {
				window.requestAnimationFrame(() => {
					this.handleScroll();
					this.ticking = false;
				});
				this.ticking = true;
			}
		});
	}
});