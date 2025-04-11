function i(t){return t===Object(t)}function e(t){return JSON.parse(JSON.stringify({...t,__isCopied__:!0}))}var a=(t)=>i(t)?e(t):t,l={log:(t)=>console.log(a(t)),info:(t)=>console.info(a(t)),warn:(t)=>console.warn(a(t)),error:(t)=>console.error(a(t)),debug:(t)=>console.debug(a(t))};Vue.component("input-blog-tags",{props:{max:{type:Number,required:!0},label:{type:String,required:!0},value:{type:String,default:""},desc:{type:String,required:!1,default:null},validateMsg:{type:String,default:null}},data:function(){return l.log(this.value),{text:this.value,name:this.label.replace(/\s+/g,"")}},computed:{countTags:function(){return this.text?this.text.split(",").length:0},validate:function(){return this.text.split(",").length<=this.max},validationString:function(){return this.validateMsg.replace("{0}",this.label).replace("{1}",`${this.max}`)}},template:`
        <div class="o-form-group">
        <label :for="name">{{ label.replace(/([A-Z])/g, " $1") }}</label>
        <p class="desc" v-if="desc">{{ desc }}</p>
        <input :name="name"
               :id="name"
               type="text"
               class="o-form-control active-border"
               v-model="text">
        <div class="counter" :class="validate ? '' : 'invalid'">
            <div class="o-progress-bar" :style="{ width: Math.min(100, 100 * (countTags / max)) + '%' }"></div>
            <span>{{ countTags }}/{{ max.toLocaleString() }}</span>
        </div>
        <span v-if="!validate && validateMsg">{{ validationString }}</span>
        </div>
    `});

//# debugId=5C2142F90E9C386064756E2164756E21
