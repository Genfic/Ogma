(() => {
    const passwordInputs = [...document.querySelectorAll('input[type=password]')] as HTMLInputElement[];

    for (const pi of passwordInputs) {
        const buttons = pi.nextElementSibling;

        if (buttons === null) break;

        buttons.querySelector('.show-password').addEventListener('click', e => {
            e.preventDefault();
            if (pi.type === 'password') {
                pi.type = 'text';
                (e.currentTarget as HTMLElement).querySelector('i').innerText = 'visibility';
            } else {
                pi.type = 'password';
                (e.currentTarget as HTMLElement).querySelector('i').innerText = 'visibility_off';
            }
        });
    }
})();