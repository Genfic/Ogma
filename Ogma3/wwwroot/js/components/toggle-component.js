Vue.component("input-toggle",{props:{label:{type:String,required:!0},desc:{type:String,default:null},value:{type:Boolean,default:!1}},data:function(){return{name:this.label.replace(/\s+/g,""),checked:this.value}},methods:{update:function(e){this.$emit("input",e)}},watch:{value(){this.checked=this.value}},template:`
        <div class="o-form-group keep-size">
        <label>{{ label.replace(/([A-Z])/g, " $1") }}</label>
        <p class="desc" v-if="desc">{{ desc }}</p>

        <div class="toggle-input">
            <input type="checkbox"
                   :name="name"
                   :id="name"
                   @change="update($event.target.checked)"
                   v-model="checked"
                   value="true">

            <label :for="name">
                    <span class="toggle">
                        <span class="dot"></span>
                    </span>

                {{ checked ? 'On' : 'Off' }}
            </label>

        </div>
        </div>
    `});

//# debugId=384BAC96F8F8415064756E2164756E21
//# sourceMappingURL=toggle-component.js.map
