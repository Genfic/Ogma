(function(){
    
    window.customElements.define('report-button', class ReportButton extends HTMLElement {
        constructor() {
            super();
            
            this.innerHTML = `
                <button class="btn action-btn report" title="Report">
                    <i class="icon material-icons-outlined">flag</i>
                </button>
            `;
        }
        
        connectedCallback() {
            this.querySelector('button').addEventListener('click', e => {
                console.log('Clicked!', this.type, this.itemId, this.route);
            });
        }

        static get observedAttributes() {
            return ['type', 'item-id', 'route'];
        }

        get type() {
            return this.getAttribute('type');
        }
        set type(value) {
            this.setAttribute('type', value);
        }

        get itemId() {
            return this.getAttribute('item-id');
        }
        set itemId(value) {
            this.setAttribute('item-id', value);
        }

        get route() {
            return this.getAttribute('route');
        }
        set route(value) {
            this.setAttribute('route', value);
        }
    });
})();