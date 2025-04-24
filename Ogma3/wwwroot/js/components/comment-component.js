var C="GET";var y="PATCH",h="DELETE";async function p(e,t,n,a,i){let o=await fetch(e,{method:t,headers:{"content-type":"application/json",...a},body:n&&JSON.stringify(n),...i}),c=o.headers.get("content-type")?.includes("application/json")?await o.json():await o.text();return{ok:o.ok,status:o.status,statusText:o.statusText,headers:o.headers,data:c}}var b=async(e,t,n)=>await p(`/api/comments/${e}`,h,void 0,t,n);var q=async(e,t,n)=>await p(`/api/comments/${e}/md`,C,void 0,t,n),A=async(e,t,n)=>await p(`/api/comments/${e}/revisions`,C,void 0,t,n);var H=async(e,t,n)=>await p("/api/comments",y,e,t,n);var g={MMMM:1,MM:2,Mo:12,YYYY:3,YY:4,dddd:5,DD:11,Do:10,h:6,H:13,mm:7,ss:8,a:9},x=["January","February","March","April","May","June","July","August","September","October","November","December"],S=["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"];function d(e){return e<10?`0${e}`:`${e}`}function D(e){let t=["th","st","nd","rd"],n=e%100,a=t[(n-20)%10]||t[n]||t[0];return`${e}${a}`}function T(e,t,n){let a=t.getMonth(),i=t.getFullYear(),o=t.getHours(),r=t.getSeconds(),c=t.getMinutes(),l=t.getDate(),s="",I=0;while(I<e.length){let R=e[I];({[0]:()=>{s+=R[1]},[10]:()=>{s+=D(l)},[2]:()=>{s+=x[a]?.slice(0,3)},[1]:()=>{s+=x[a]},[12]:()=>{let u=a+1;s+=n.padMonth?d(u):`${u}`},[3]:()=>{s+=i},[4]:()=>{s+=`${i%100}`},[5]:()=>{s+=S[t.getDay()]},[11]:()=>{s+=n.padDays?d(l):l},[6]:()=>{let u=o%12||12;s+=n.padHours?d(u):u},[13]:()=>{s+=n.padHours?d(o):o},[7]:()=>{s+=d(c)},[8]:()=>{s+=d(r)},[9]:()=>{s+=o>=12?"PM":"AM"}})[R[0]](),I++}return s}function v(e){let t=[],n=0,a="";while(n<e.length){let i=e[n++];if(i==="{"){if(a)t.push([0,a]);a="";let o="";i=e[n++];while(i!=="}")o+=i,i=e[n++];let r=g[o];if(!r)throw new Error(`Unknown substitution: ${o}`);t.push([r])}else a+=i}if(a)t.push([0,a]);return t}var m=(e,t={})=>{let n=v(e);return{render(a){return T(n,a,t)}}};var f={padDays:!0,padMonth:!0,padHours:!0},B=m("{YYYY}-{MM}-{DD} {H}:{mm}:{ss}",f),J=m("{DD}.{Mo}.{YYYY} {H}:{mm}",f),w=m("{Do} {MMMM} {YYYY}, {H}:{mm}",f);Vue.component("comment",{props:{comment:{type:Object,required:!0},idx:{type:Number,required:!0},csrf:{type:String,required:!0},highlight:{type:Boolean,required:!1,default:!1},authenticatedAs:{type:String,default:!1}},data:function(){return{editData:null,mutComment:{...this.comment,owned:this.comment.author?.userName.toLowerCase()===this.authenticatedAs},revisions:[],revisionsCache:null,hide:this.comment.isBlocked}},methods:{del:async function(){if(confirm("Are you sure you want to delete?")){if(!(await b(this.comment.id,{RequestVerificationToken:this.csrf})).ok)return;this.mutComment={...this.mutComment,deletedBy:"User"}}},edit:async function(){if(this.editData&&this.editData.id===this.comment.id)return;this.editData=null;let e=await q(this.comment.id);if(!e.ok)return;this.editData={id:this.comment.id,body:e.data}},update:async function(e){e.preventDefault();let t=await H({body:this.editData.body,commentId:Number(this.editData.id)},{RequestVerificationToken:this.csrf});if(!t.ok)return;let n=t.data;Object.assign(this.mutComment,n),this.editData=null},report:function(){this.$emit("report",this.comment.id)},enter:async function(e){if(e.ctrlKey)await this.update(e)},history:async function(){if(this.revisions.length>0)this.revisions=[];else if(this.revisionsCache!==null)this.revisions=this.revisionsCache;else{let e=await A(this.comment.id);if(!e.ok)return;this.revisionsCache=this.revisions=e.data}},changeHighlight:function(e){e.preventDefault(),this.$emit("change-hl",this.idx+1)},toggleShow:function(){if(this.comment.isBlocked)this.hide=!this.hide},date:(e)=>w.render(new Date(e))},template:`
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

//# debugId=100A7AA59A33DD7364756E2164756E21
