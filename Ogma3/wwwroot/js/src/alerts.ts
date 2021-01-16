(_ => {
    const alerts: HTMLCollectionOf<Element> = document.getElementsByClassName('alert-dismissible');
    
    for (const a of alerts) {
        a.querySelector('button.close').addEventListener('click', _ => {
            a.remove()
        })
    }
})()