new Vue({
    el: '#profile-bar',
    data: {
        route: null,
        name: null,
        
        isBlocked: null,
        isFollowed: null,
        
        xcsrf: null,
        
        done: false,
    },
    methods: {
        follow: function () {
            axios.post(this.route + '/follow',
                { name: this.name },
                {
                    headers: { 'RequestVerificationToken': this.xcsrf }
                }
            )
                .then(res => this.isFollowed = res.data)
                .catch(console.error)
        },
        block: function () {
            axios.post(this.route + '/block',
                { name: this.name }, 
                {
                    headers: { 'RequestVerificationToken': this.xcsrf }
                }
            )
            .then(res => this.isBlocked = res.data)
            .catch(console.error)
        }
    },
    mounted() {
        this.route  = document.getElementById('data-route').dataset.route;
        this.name   = document.getElementById('data-name').dataset.name; 
        
        this.isBlocked  = document.getElementById('data-blocked').dataset.blocked.toLowerCase() === 'true';
        this.isFollowed = document.getElementById('data-followed').dataset.followed.toLowerCase() === 'true';
        
        this.xcsrf  = document.querySelector('[name=__RequestVerificationToken]').value;
        this.done = true;
    }
});