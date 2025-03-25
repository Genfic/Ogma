async function m(e,t,n,a,o){let s=await fetch(e,{method:t,headers:{"Content-Type":"application/json",...a},body:n?JSON.stringify(n):null,...o}),r=s.headers.get("content-type"),u;if(r?.includes("application/json"))u=await s.json();else u=await s.text();return{ok:s.ok,status:s.status,statusText:s.statusText,headers:s.headers,data:u}}var C=async(e,t,n)=>await m(`/api/comments/${e}`,"DELETE",void 0,t,n);var y=async(e,t,n)=>await m(`/api/comments/${e}/md`,"GET",void 0,t,n),f=async(e,t,n)=>await m(`/api/comments/${e}/revisions`,"GET",void 0,t,n);var b=async(e,t,n)=>await m("/api/comments","PATCH",e,t,n);var A={MMMM:1,MM:2,Mo:12,YYYY:3,YY:4,dddd:5,DD:11,Do:10,h:6,H:13,mm:7,ss:8,a:9},q=["January","February","March","April","May","June","July","August","September","October","November","December"],w=["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"];function d(e){return e<10?`0${e}`:`${e}`}function H(e){let t=["th","st","nd","rd"],n=e%100,a=t[(n-20)%10]||t[n]||t[0];return`${e}${a}`}function x(e,t,n){let a=t.getMonth(),o=t.getFullYear(),s=t.getHours(),r=t.getSeconds(),u=t.getMinutes(),l=t.getDate(),i="",I=0;while(I<e.length){let T=e[I];({[0]:()=>{i+=T[1]},[10]:()=>{i+=H(l)},[2]:()=>{i+=q[a]?.slice(0,3)},[1]:()=>{i+=q[a]},[12]:()=>{let p=a+1;i+=n.padMonth?d(p):`${p}`},[3]:()=>{i+=o},[4]:()=>{i+=`${o%100}`},[5]:()=>{i+=w[t.getDay()]},[11]:()=>{i+=n.padDays?d(l):l},[6]:()=>{let p=s%12||12;i+=n.padHours?d(p):p},[13]:()=>{i+=n.padHours?d(s):s},[7]:()=>{i+=d(u)},[8]:()=>{i+=d(r)},[9]:()=>{i+=s>=12?"PM":"AM"}})[T[0]](),I++}return i}function g(e){let t=[],n=0,a="";while(n<e.length){let o=e[n++];if(o==="{"){if(a)t.push([0,a]);a="";let s="";o=e[n++];while(o!=="}")s+=o,o=e[n++];let r=A[s];if(!r)throw new Error(`Unknown substitution: ${s}`);t.push([r])}else a+=o}if(a)t.push([0,a]);return t}var c=(e,t={})=>{let n=g(e);return{render(a){return x(n,a,t)}}};var R={padDays:!0,padMonth:!0,padHours:!0},U=c("{YYYY}-{MM}-{DD} {H}:{mm}:{ss}",R),$=c("{DD}.{MM}.{YYYY} {H}:{mm}",R),h=c("{Do} {MMMM} {YYYY}, {H}:{mm}",R);Vue.component("comment",{props:{comment:{type:Object,required:!0},idx:{type:Number,required:!0},csrf:{type:String,required:!0},highlight:{type:Boolean,required:!1,default:!1},authenticatedAs:{type:String,default:!1}},data:function(){return{editData:null,mutComment:{...this.comment,owned:this.comment.author?.userName.toLowerCase()===this.authenticatedAs},revisions:[],revisionsCache:null,hide:this.comment.isBlocked}},methods:{del:async function(){if(confirm("Are you sure you want to delete?")){if(!(await C(this.comment.id,{RequestVerificationToken:this.csrf})).ok)return;this.mutComment={...this.mutComment,deletedBy:"User"}}},edit:async function(){if(this.editData&&this.editData.id===this.comment.id)return;this.editData=null;let e=await y(this.comment.id);if(!e.ok)return;this.editData={id:this.comment.id,body:e.data}},update:async function(e){e.preventDefault();let t=await b({body:this.editData.body,commentId:Number(this.editData.id)},{RequestVerificationToken:this.csrf});if(!t.ok)return;let n=t.data;Object.assign(this.mutComment,n),this.editData=null},report:function(){this.$emit("report",this.comment.id)},enter:async function(e){if(e.ctrlKey)await this.update(e)},history:async function(){if(this.revisions.length>0)this.revisions=[];else if(this.revisionsCache!==null)this.revisions=this.revisionsCache;else{let e=await f(this.comment.id);if(!e.ok)return;this.revisionsCache=this.revisions=e.data}},changeHighlight:function(e){e.preventDefault(),this.$emit("change-hl",this.idx+1)},toggleShow:function(){if(this.comment.isBlocked)this.hide=!this.hide},date:(e)=>h.render(new Date(e))},template:`
        <div :id="'comment-' + (idx + 1)"
             class="comment" :class="highlight ? 'marked' : ''">

        <!-- Blocked comment -->
        <template v-if="this.hide">
            <div class="main" v-on:click="toggleShow">
                <div class="header">
                    Comment hidden by user blacklist
                </div>
            </div>
        </template>

        <!-- Deleted comment -->
        <template v-else-if="mutComment.deletedBy">
            <div class="main">
                <div class="header">

                    <time :datetime="mutComment.dateTime" class="time">{{ date(mutComment.dateTime) }}</time>
                    <p class="sm-line"></p>
                    <span>Comment deleted by {{ mutComment.deletedBy.toLowerCase() }}.</span>

                </div>
            </div>
        </template>

        <!-- Regular comment -->
        <template v-else>
            <div class="author">

                <a :href="'/user/' + mutComment.author.userName" class="name">{{ mutComment.author.userName }}</a>

                <div v-if="mutComment.author.roles[0]" class="role-tag">
                    <span class="name">{{ mutComment.author.roles[0].name }}</span>
                    <div class="bg" :style="{backgroundColor: mutComment.author.roles[0].color}"></div>
                </div>

                <img :src="mutComment.author.avatar" :alt="mutComment.author.userName + '\\'s avatar'"
                     class="avatar"
                     loading="lazy">

            </div>

            <div class="main" :class="comment.isBlocked ? 'blocked' : null">

                <div class="header">

                    <a class="link"
                       :href="'#comment-' + (idx + 1)"
                       v-on:click="changeHighlight($event)">
                        #{{ idx + 1 }}
                    </a>

                    <p class="sm-line"></p>

                    <time :datetime="mutComment.dateTime" class="time" v-on:click="toggleShow">
                        {{ date(mutComment.dateTime) }}
                    </time>

                    <div v-if="authenticatedAs" class="actions">

                        <button class="action-btn small red-hl" title="Report" v-on:click="report">
                            <o-icon icon="lucide:flag" class="material-icons-outlined icon" ></o-icon>
                        </button>

                        <template v-if="mutComment.owned">

                            <button class="action-btn small" title="Delete" v-on:click="del">
                            	<o-icon icon="lucide:trash-2" class="material-icons-outlined icon" ></o-icon>
                            </button>

                            <button class="action-btn small" title="Edit" v-on:click="edit">
                            	<o-icon icon="lucide:pencil" class="material-icons-outlined icon" ></o-icon>
                            </button>

                        </template>
                    </div>

                </div>

                <div v-if="mutComment.body && (!editData || editData.id !== mutComment.id)" class="body md"
                     v-html="mutComment.body"></div>

                <form class="form" v-if="editData && editData.id === mutComment.id">
            <textarea class="comment-box"
                      v-model="editData.body"
                      v-on:keydown.enter="enter"
                      name="body" id="edit-body"
                      rows="3"
                      aria-label="Comment">
            </textarea>

                    <div class="buttons">
                        <button class="confirm active-border" v-on:click="update">
            				<o-icon icon="lucide:pencil" class="material-icons-outlined" ></o-icon>
                            Update
                        </button>
                        <button class="cancel active-border" v-on:click="editData = null">
            				<o-icon icon="lucide:x" class="material-icons-outlined" ></o-icon>
                            Cancel
                        </button>
                    </div>
                </form>

                <button v-if="mutComment.isEdited" v-on:click="history" class="edit-data">Edited</button>

                <ol v-if="revisions.length > 0" class="history">
                    <li v-for="r in revisions">
                        <time :datetime="r.editTime">{{ date(r.editTime) }}</time>
                        <div class="body" v-html="r.body"></div>
                    </li>
                </ol>

            </div>
        </template>
        </div>
    `});

//# debugId=ED0F4C012D6D9E0564756E2164756E21
//# sourceMappingURL=comment-component.js.map
