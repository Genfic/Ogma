(function(){
    
    window.customElements.define('api-button', class ReportButton extends HTMLElement {
        constructor() {
            super();
            
            this.innerHTML = `
                <div style="min-height: 1rem">
                    <slot>Click</slot>
                </div>
            `;
        }
        
        connectedCallback() {
            this.addEventListener('click', _ => {
                
                let body = ['get', 'head'].indexOf(this.method.toLowerCase()) === -1
                    ? JSON.stringify(this.dataset)
                    : null;
                
                fetch(this.url, {
                    method: this.method ?? 'post',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body
                })
                .then(_ => this.classList.add('success'))
                .catch(err => {
                    console.error(err);
                    this.classList.add('error');
                })
                .finally(_ => console.info('Fetched!'))
            });
        }

        static get observedAttributes() {
            return ['url', 'method'];
        }

        get url() {
            return this.getAttribute('url');
        }
        set url(value) {
            this.setAttribute('url', value);
        }

        get method() {
            return this.getAttribute('method');
        }
        set method(value) {
            this.setAttribute('method', value);
        }
    });
})();