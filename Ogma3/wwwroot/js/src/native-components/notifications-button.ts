(_ => {
    window.customElements.define('o-notifications', class extends HTMLElement {
               
        constructor() {
            super();

            this.innerHTML = 
                `<a class="nav-link light" href="${this.href}" title="Notifications">
                    <i class="material-icons-outlined">notifications</i>
                </a>`;
        }
        
        public connectedCallback() {
            this.load()
        }
        
        public load() {
            fetch(this.endpoint)
                .then(data => data.json())
                .then((json: number) => {
                    if (json > 0) {
                        let badge = document.createElement('span');
                        badge.innerText = json.toString();
                        badge.classList.add('badge');
                        this.querySelector('a.nav-link').appendChild(badge);
                    }
                })
                .catch(console.error)
        }
        
        get endpoint() {
            return this.getAttribute('endpoint');
        }
        
        get href() {
            return this.getAttribute('href');
        }
        
    });
})();