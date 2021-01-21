interface Quote {
    body: string;
    author: string;
}

(_ => {
    window.customElements.define('o-quotebox', class extends HTMLElement {
               
        constructor() {
            super();

            this.innerHTML = 
                `<div id="quote" class="quote active-border">
                    <div class="refresh">
                        <i class="material-icons-outlined">refresh</i>
                    </div>
                    <em class="body"></em>
                    <span class="author"></span>
                </div>`;
        }
        
        public connectedCallback() {
            this.update()
                        
            this.querySelector('.refresh').addEventListener('click', _ => {
                this.update()
            })
        }
        
        private update() : void {
            this.querySelector('i.material-icons-outlined').classList.add('spin');
            fetch(this.endpoint)
                .then(data => data.json())
                .then((json: Quote) => {
                    this.querySelector('em.body').innerHTML = json.body;
                    this.querySelector('span.author').innerHTML = json.author;
                })
                .catch(console.error)
                .finally(() => {
                    this.querySelector('i.material-icons-outlined').classList.remove('spin');
                })
        }
        
        get endpoint() {
            return this.getAttribute('endpoint');
        }
        
    });
})();