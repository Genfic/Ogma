function sendForm() {
    const inputs = [
        document.getElementById('query'),
        document.getElementById('rating'),
        document.getElementById('sort'),
    ];
    for (let e of inputs) {
        if (e.value === null || e.value === '') e.disabled = true;
    }
}

(function () {
    document
        .getElementById('search')
        .addEventListener('submit', sendForm);
})()

const search_vue = new Vue({
   el: '#search' 
});