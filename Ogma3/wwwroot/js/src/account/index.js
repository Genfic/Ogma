new Vue({
	el: '#app',
	data: {
		hasImage: false 
	},
	methods: {
		setImage: function(output) {
			this.hasImage = true;
			console.log(output);
		},
	},
});