(_ => {
    const passwordInputs = [...document.querySelectorAll('input[type=password]')];
    for (const pi of passwordInputs) {
        const buttons = pi.nextElementSibling;
        if (buttons === null)
            break;
        buttons.querySelector('.show-password').addEventListener('click', e => {
            e.preventDefault();
            if (pi.type === 'password') {
                pi.type = 'text';
                e.currentTarget.querySelector('i').innerText = 'visibility';
            }
            else {
                pi.type = 'password';
                e.currentTarget.querySelector('i').innerText = 'visibility_off';
            }
        });
    }
})();
