Vue.component("textarea-counter",{props:{max:{type:Number,required:!0},min:{type:Number,default:0},label:{type:String,required:!0},value:{type:String,default:""},rows:{type:Number,default:5},desc:{type:String,default:null},validateMsg:{type:String,default:null},wordcount:{type:Boolean,default:!1}},data:function(){return{text:this.value,name:this.label.replace(/\s+/g,"")}},computed:{countChars:function(){return this.text.length.toLocaleString()},countWords:function(){return this.text.replace(/[\W_]+/," ").split(/\s+/).length.toLocaleString()},validate:function(){return this.text.length>=this.min&&this.text.length<=this.max},validationString:function(){return this.validateMsg.replace("{0}",this.label).replace("{1}",`${this.max}`).replace("{2}",`${this.min}`)}},methods:{updateValue:function(){this.$emit("input",this.text)},clear:function(){this.text=""}},template:`
        <div class="o-form-group">
        <label :for="name">{{ label.replace(/([A-Z])/g, " $1") }}</label>

        <p class="desc" v-if="desc">{{ desc }}</p>

        <textarea :name="name"
                  :id="name"
                  class="o-form-control active-border"
                  v-model="text"
                  v-on:input="updateValue"
                  :rows="rows">
            </textarea>

        <div class="counter" :class="validate ? '' : 'invalid'">
            <div class="o-progress-bar" :style="{ width: Math.min(100, 100 * (countChars / max)) + '%' }"></div>
            <span v-if="wordcount">{{ countWords }} words, </span>
            <span>{{ countChars }}/{{ max.toLocaleString() }}<span v-if="wordcount"> chars</span></span>
        </div>

        <span v-if="!validate && validateMsg">{{ validationString }}</span>
        </div>
    `});

//# debugId=C56BE90142E0A5EF64756E2164756E21
//# sourceMappingURL=textarea-counter-component.js.map
