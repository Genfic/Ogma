Vue.component("input-file",{props:{max:{type:Number,required:!0},label:{type:String,required:!0},desc:{type:String,required:!1,default:null},validateMsg:{type:String,default:null}},data:function(){return{name:this.label.replace(/\s+/g,""),file:null}},methods:{fileSelected(e){this.file=e.target.files[0]}},computed:{validate:function(){if(this.file)return this.file.size<=this.max;return!0},validationString:function(){return this.validateMsg.replace("{0}",this.label).replace("{1}",`${this.max}`)}},template:`
        <div class="o-form-group">
        <label :for="name">{{ label.replace(/([A-Z])/g, " $1") }}</label>
        <p class="desc" v-if="desc">{{ desc }}</p>
        <input :name="name"
               :id="name"
               type="file"
               ref="file"
               @change="fileSelected"
               class="o-form-control active-border"
               accept=".png,.jpg,.jpeg,.webp">
        <div class="counter" :class="validate ? '' : 'invalid'">
            <div class="o-progress-bar"
                 :style="{ width: Math.min(100, 100 * ((file ? file.size : 0) / max)) + '%' }"></div>
            <span>{{ file ? file.size : 0 }}/{{ max }} bytes</span>
        </div>
        <span v-if="!validate && validateMsg">{{ validationString }}</span>
        </div>
    `});

//# debugId=FE158332FA266EFB64756E2164756E21
//# sourceMappingURL=input-file-component.js.map
