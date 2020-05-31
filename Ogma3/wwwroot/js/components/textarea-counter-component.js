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
        cssClass: function () {
            return this.text.length >= this.min && this.text.length <= this.max ? '' : 'invalid'
        }
    },
    template: `
        <div class="o-form-group">
            <label for="">{{label}}</label>
            <textarea class="o-form-control active-border" v-model="text" :rows="rows"></textarea>
            <span class="counter" :class="cssClass">{{countChars}}/{{max}}</span> 
        </div>
    `
});