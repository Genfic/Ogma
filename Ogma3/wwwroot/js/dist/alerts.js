(_ => {
    const alerts = document.getElementsByClassName('alert-dismissible');
    for (const a of alerts) {
        a.querySelector('button.close').addEventListener('click', _ => {
            a.remove();
        });
    }
})();
