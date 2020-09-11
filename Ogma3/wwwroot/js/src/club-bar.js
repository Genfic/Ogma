const club_bar_vue = new Vue({
    el: '#club-bar',
    data: {
        joined: null,
        route: null,
        id: null
    },
    methods: {
        join: function () {
            axios.post(this.route, {clubId: this.id})
                .then(res => this.joined = res.data)
                .catch(console.error)
        }
    },
    computed: {
        getClass: function () {
            switch (this.joined) {
                case null:
                    return '';
                case true:
                    return 'leave';
                case false:
                    return 'join';
            }
        },
        getText: function () {
            switch (this.joined) {
                case null:
                    return 'Checking...';
                case true:
                    return 'Leave club';
                case false:
                    return 'Join club';
            }
        }
    },
    mounted() {
        this.route = document.getElementById('data-route').dataset.route;
        this.id = Number(document.getElementById('data-id').dataset.id);

        axios.get(this.route + '/' + this.id)
            .then(res => this.joined = res.data)
            .catch(console.error)
    }
})