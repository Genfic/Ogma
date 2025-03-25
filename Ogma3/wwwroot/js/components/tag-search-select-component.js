async function o(e,t,s,a,u){let n=await fetch(e,{method:t,headers:{"Content-Type":"application/json",...a},body:s?JSON.stringify(s):null,...u}),p=n.headers.get("content-type"),i;if(p?.includes("application/json"))i=await n.json();else i=await n.text();return{ok:n.ok,status:n.status,statusText:n.statusText,headers:n.headers,data:i}}var r=async(e,t)=>await o("/api/tags/all","GET",void 0,e,t);var d=async(e,t,s)=>await o(`/api/tags/story/${e}`,"GET",void 0,t,s);Vue.component("tag-search-select",{props:{min:{type:Number,default:0},label:{type:String,required:!0},desc:{type:String,required:!1,default:null},validateMsg:{type:String,default:null},storyId:{type:Number,default:null},preselected:{type:Array,default:null},inline:{type:Boolean,default:!1},disableWhenEmpty:{type:Boolean,default:!1},hideLabels:{type:Boolean,default:!1}},data:function(){return{name:this.label.replace(/\s+/g,"").toLowerCase(),loading:!0,options:[],selected:[],search:"",highlighted:null,focused:!1,disable:!1}},computed:{validate:function(){return this.selected.length>=this.min},validationString:function(){return this.validateMsg.replace("{0}",`${this.min}`)},filtered(){return this.options.filter((e)=>{let t=e.name.toLowerCase().includes(this.search.toLowerCase()),s=e.namespace?.toLowerCase().includes(this.search.toLowerCase())??!1;return(t||s)&&!this.selected.some((a)=>a.id===e.id)&&this.search.length>0})}},methods:{pushUnique:(e,t)=>{if(e.includes(t))return;e.push(t)},handleInputKeys:function(e){switch(e.key){case"Backspace":if(this.search.length<=0)this.selected.pop();break;case"ArrowUp":if(this.highlighted!==null)e.preventDefault();if(this.highlighted>0)this.highlighted--;else this.highlighted=null;break;case"ArrowDown":if(this.highlighted!==null)e.preventDefault();if(this.highlighted===null)this.highlighted=0;else if(this.highlighted<this.filtered.length-1)this.highlighted++;break;case" ":case"Enter":if(this.highlighted!==null)e.preventDefault(),this.highlighted=0,this.pushUnique(this.selected,JSON.parse(JSON.stringify(this.filtered[this.highlighted])));break;default:break}},checkDisabled:function(){return this.disable=this.selected.length<=0,this.disable},onClose:function(){this.focused=!1}},async created(){let e=await r();if(this.options=e.data,this.loading=!1,this.storyId){let t=await d(this.storyId);this.selected=t.data;for(let s of this.selected)this.options.find((a)=>a.id===s.id).hidden=!0}if(this.preselected)this.selected=this.options.filter((t)=>this.preselected.indexOf(t.id)!==-1)},template:`
        <div class="tag-search-select"
             v-on:focusin="focused = true">
        <select class="output"
                :name="name"
                multiple="multiple"
                :id="name"
                :disabled="disable">
            <option v-for="s in selected" :value="s.id" selected>{{ s.name }}</option>
        </select>

        <div class="o-form-group tag-search"
             :class="inline ? 'inline' : null"
             :style="{ marginTop: hideLabels ? 0 : null }"
             v-closable="{
                    exclude: ['search'],
                    handler: 'onClose'
                 }">
            <template v-if="!hideLabels">
                <label :for="name">{{ label.replace(/([A-Z])/g, " $1") }}</label>
                <p class="desc" v-if="desc && !inline">{{ desc }}</p>
            </template>

            <div class="searchbar" ref="search">
                <div class="tags">

                    <div class="tag" v-for="s in selected">
                        <div class="bg" v-bind:style="{background: s.namespaceColor}"></div>
                        <span class="name">
                        {{ s.namespaceName ? s.namespaceName + ':' : '' }}{{ s.name }}
            				<o-icon icon="lucide:x" class="material-icons-outlined" v-on:click="selected.remove(s)" ></o-icon>
                      </span>
                    </div>

                    <input type="text"
                           class="search"
                           v-model="search"
                           v-on:keydown="handleInputKeys"
                           placeholder="Search...">
                </div>

                <ol v-if="!loading && focused" class="search-results">
                    <li v-for="(o, idx) in filtered"
                        :style="{background: o.rgba}"
                        :class="highlighted === idx ? 'hl' : null"
                        v-on:click="selected.pushUnique(o)">
                        <span class="name">{{ o.namespace ? o.namespace + ':' : '' }}{{ o.name }}</span>
                    </li>
                </ol>
            </div>

        </div>

        <span v-if="!validate && validateMsg">{{ validationString }}</span>
        </div>
    `});

//# debugId=C035ACC40EBED3BE64756E2164756E21
//# sourceMappingURL=tag-search-select-component.js.map
