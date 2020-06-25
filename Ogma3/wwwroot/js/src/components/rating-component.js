Vue.component('input-rating', {
    props: {
        label: {
            type: String,
            required: true
        },
        desc: {
            type: String,
            required: false,
            default: null
        },
        ratingsApi: {
            type: String,
            required: true
        }
    },
    data: function () {
        return {
            name: this.label.replace(/\s+/g, ''),
            loading: true,
            ratings: []
        }
    },
    created() {
        axios.get(this.ratingsApi)
            .then(res => {
                this.ratings = res.data;
                this.loading = false;
            })
            .catch(console.error);
    },
    template: `
        <div class="o-form-group">
            <label :for="name">{{label}}</label>
            <p class="desc" v-if="desc">{{desc}}</p>

            <div v-if="loading">Loading ratings...</div>
            
            <div v-if="!loading" class="ratings">
                <div v-for="(rating, idx) in ratings" class="rating">
   
                    <input type="radio"
                           :name="name"
                           :id="rating.name.toLowerCase()"
                           :value="rating.id" 
                           :checked="idx === 0"
                           class="radio">
                    
                    <label class="radio-label active-border" :for="rating.name.toLowerCase()">
                        <img :src="rating.icon" alt="">
                        <div class="main">
                            <strong>{{rating.name}}</strong>
                            <span>{{rating.description}}</span>
                        </div>
                    </label>
                </div>
            </div>
        </div>
    `
});