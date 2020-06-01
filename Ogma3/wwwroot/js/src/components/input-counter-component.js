Vue.component('input-counter', {
    props: {
        max: {
            type: Number,
            required: true
        },
        min: {
            type: Number,
            default: 0
        },
        label: {
            type: String,
            required: true
        },
        value: {
            type: String,
            default: ''
        },
        name: {
            type: String,
            required: true
        },
        validateMsg: {
            type: String,
            default: null
        }
    },
    data: function () {
        return {
            text: this.value
        }
    },
    computed: {
        countChars: function () {
            return this.text.length;
        },
        validate: function () {
            return this.text.length >= this.min && this.text.length <= this.max
        },
        validationString: function () {
            return this.validateMsg
                .replace('{0}', this.label)
                .replace('{1}', this.max)
                .replace('{2}', this.min);
        }
    },
    template: `
        <div class="o-form-group">
            <label :for="name">{{label}}</label>
            <input  :name="name"
                    :id="name" 
                    type="text" 
                    class="o-form-control active-border" 
                    v-model="text">
            <span class="counter" :class="validate ? '' : 'invalid'">{{countChars}}/{{max}}</span>
            <span v-if="!validate && validateMsg">{{validationString}}</span>
        </div>
    ` 
});