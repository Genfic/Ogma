// Set Vue error handling
// @ts-ignore
Vue.config.errorHandler = function(err) {
	console.info(err.message); // "Oops"
};
// @ts-ignore
Vue.config.ignoredElements = [/o-*/];