Vue.component("status-select",{props:{label:{type:String,required:!0},value:{type:Number},desc:{type:String},statusesStr:{type:String,required:!0}},data:function(){return{statuses:JSON.parse(this.statusesStr),name:this.label.replace(/\s+/g,""),selected:this.value??0}},methods:{},template:`
        <div class="o-form-group status-select">
        <label>{{ label.replace(/([A-Z])/g, " $1") }}</label>
        <p class="desc" v-if="desc">{{ desc }}</p>

        <div class="selector active-border">
            <template v-for="(r, k) in statuses">
                <input type="radio"
                       :id="'sel-' + r"
                       :name="name"
                       :value='k + 1'
                       v-model="selected">
                <label :for="'sel-' + r" tabindex="0">{{ r }}</label>
            </template>
        </div>
        </div>
    `});

//# debugId=79C974A75F76E56C64756E2164756E21
