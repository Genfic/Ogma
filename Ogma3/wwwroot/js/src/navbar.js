(() => {
    let last_known_scroll_position = 0;
    let ticking = false;
    const nav = document.getElementById('top-nav');

    let lastPos = 0;
    function changeNav(pos) {
        if (pos - lastPos > 0) {
            nav.classList.add('compact');
        } else {
            nav.classList.remove('compact');
        }
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