Vue.component('textarea-counter', {
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
        rows: {
            type: Number,
            default: 5
        },
        validateMsg: {
            type: String,
            default: null
        }
    },
    data: function () {
        return {
            text: this.value,
            name: this.label.replace(/\s+/g, '')
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
            <textarea 
                    :name="name"
                    :id="name" 
                    class="o-form-control active-border" 
                    v-model="text" 
                    :rows="rows">
            </textarea>
            <span class="counter" :class="validate ? '' : 'invalid'">{{countChars}}/{{max}}</span>
            <span v-if="!validate && validateMsg">{{validationString}}</span>
        </div>
    `
});