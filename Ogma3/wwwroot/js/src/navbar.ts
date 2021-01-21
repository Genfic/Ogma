(_ => {
    let last_known_scroll_position: number = 0;
    let ticking: boolean = false;
    const nav: HTMLElement = document.getElementById('top-nav');

    let lastPos: number = 0;
    function changeNav(pos) {
        nav.classList.toggle('compact', pos - lastPos > 0)
        lastPos = pos;
    }

    window.addEventListener('scroll', _ => {
        last_known_scroll_position = window.scrollY;

        if (!ticking) {
            window.requestAnimationFrame(() => {
                changeNav(last_known_scroll_position);
                ticking = false;
            });
            ticking = true;
        }
    });
})()