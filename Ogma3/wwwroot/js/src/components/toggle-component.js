Vue.component('input-toggle', {
    props: {
        label: {
            type: String,
            required: true
        },
        desc: {
            type: String,
            default: null
        },
        value: {
            type: Boolean,
            default: false
        }
    },
    data: function () {
        return {
            name: this.label.replace(/\s+/g, ''),
            checked: this.value,
        }
    },
    template: `
        <div class="o-form-group">
            <label>{{label.replace( /([A-Z])/g, " $1" )}}</label>
            <p class="desc" v-if="desc">{{desc}}</p>

            <div class="toggle-input">
                <input type="checkbox"
                     :name="name"
                     :id="name"
                     v-model="checked"
                     value="true">
              
                <label :for="name">
                    <span class="toggle">
                        <span class="dot"></span>
                    </span>
                  
                    {{checked ? 'On' : 'Off'}}
                </label>
              
            </div>
        </div>
    `
});