(function () {
	let themeLink = document.querySelector('link#theme-ph') as HTMLLinkElement;
	let themeBtn = document.getElementById('theme-swap') as HTMLButtonElement;

	let rnd: string = Math.random()
		.toString(36)
		.replace(/[^a-z]+/g, '')
		.substr(0, 5);
	
	let date = new Date();
	
	date.setFullYear(date.getFullYear() + 100);

	themeBtn.addEventListener('click', () => {
		let theme = getCookieValue('theme') === 'dark' ? 'light' : 'dark';
        
		themeLink.setAttribute('rel', 'stylesheet');
		themeLink.setAttribute('href', `/css/${theme}.min.css?v=${rnd}`);
        
		setCookie('theme', theme, date, true, 'lax');
	});
})();