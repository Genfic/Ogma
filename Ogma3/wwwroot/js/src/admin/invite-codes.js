let anamespaces_vue = new Vue({
    el: "#app",
    data: {
        codes: [],
        route: null,
        xcsrf: null,
    },
    methods: {

        createCode: function() {
            axios.post(this.route)
                .then(response => {
                    this.codes.push(response.data)
                })
                .catch(error => {
                    console.log(error);
                });
        },

        // Gets all existing namespaces
        getCodes: function () {
            axios.get(this.route)
                .then(response => {
                    this.codes = response.data
                })
                .catch(error => {
                    console.log(error);
                });
        },

        // Deletes a selected namespace
        deleteCode: function (t) {
            if(confirm("Delete permanently?")) {
                axios.delete(this.route + '/' + t.id)
                    .then(_ => {
                        this.getCodes()
                    })
                    .catch(error => {
                        console.log(error);
                    });
            }
        },
        
        copyCode: function(t) {
            navigator.clipboard.writeText(t.code).then(
                ( ) => alert("Copied"), 
                (e) => {
                    alert("Could not copy");
                    console.error(e)
                }
            );
        },

        // Parse date
        date: function (dt) {
            return dayjs(dt).format('DD MMM YYYY, HH:mm');
        }
    }, 

    mounted() {
        // Grab the route from route helper
        this.route = document.getElementById('route').dataset.route;
        // Grab the XCSRF token
        this.xcsrf = document.querySelector('[name=__RequestVerificationToken]').value; 
        // Grab the initial set of namespaces
        this.getCodes();
    }
});