(function () {
    let themeLink = document.querySelector('link#theme-ph');
    let themeBtn = document.getElementById('theme-swap');

    let rnd = Math
        .random()
        .toString(36)
        .replace(/[^a-z]+/g, '')
        .substr(0, 5);
    let date = new Date();
    date.setFullYear(date.getFullYear() + 100);

    themeBtn.addEventListener('click', _ => {
        let theme = getCookieValue('theme') === 'dark' ? 'light' : 'dark';
        
        themeLink.setAttribute('rel', 'stylesheet');
        themeLink.setAttribute('href', `/css/${theme}.min.css?v=${rnd}`);
        
        setCookie('theme', theme, date, true, 'lax');
    });
})()