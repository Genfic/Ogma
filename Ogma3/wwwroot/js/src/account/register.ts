(_ => {
    const classname = 'visible';
    const inputs = [...document.querySelectorAll('input[id]')] as HTMLInputElement[];

    for (const input of inputs) {
        const info = document.querySelector(`[data-for="${input.id}"]`);
        if (!info) continue;

        input.addEventListener('focusin', () => {
            info.classList.add(classname)
        });
        input.addEventListener('focusout', () => {
            info.classList.remove(classname)
        });
    }
})();