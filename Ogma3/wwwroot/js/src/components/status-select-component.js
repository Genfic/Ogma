Vue.component('status-select', {
    props: {
        label: {
            type: String,
            required: true
        },
        value: {
            type: Number,
        },
        desc: {
            type: String,
        },
        statusesStr: {
            type: String,
            required: true
        }
    },
    data: function() {
        return {
            statuses: JSON.parse(this.statusesStr),
            name: this.label.replace(/\s+/g, ''),
            selected: this.value ?? 0,
        }
    },
    methods: {},
    template: `
      <div class="o-form-group status-select">
          <label>{{label.replace( /([A-Z])/g, " $1" )}}</label>
          <p class="desc" v-if="desc">{{desc}}</p>
          
          <div class="selector active-border">
            <template v-for="(r, k) in statuses">
                <input type="radio" 
                       :id="'sel-' + r" 
                       :name="name" 
                       :value='statuses.length - k'
                       v-model="selected">
                <label :for="'sel-' + r" tabindex="0">{{ r }}</label>
            </template>
          </div>
      </div>
    `
})