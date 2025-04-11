async function r(t,a,i,c,p){let e=await fetch(t,{method:a,headers:{"Content-Type":"application/json",...c},body:i?JSON.stringify(i):null,...p}),l=e.headers.get("content-type"),s;if(l?.includes("application/json"))s=await e.json();else s=await e.text();return{ok:e.ok,status:e.status,statusText:e.statusText,headers:e.headers,data:s}}var o=async(t,a,i)=>await r("/admin/api/infractions","POST",t,a,i);function u(t){return t===Object(t)}function m(t){return JSON.parse(JSON.stringify({...t,__isCopied__:!0}))}var n=(t)=>u(t)?m(t):t,d={log:(t)=>console.log(n(t)),info:(t)=>console.info(n(t)),warn:(t)=>console.warn(n(t)),error:(t)=>console.error(n(t)),debug:(t)=>console.debug(n(t))};Vue.component("manage-infraction",{props:{csrf:{type:String,required:!0},userId:{type:Number,required:!0},types:{type:Array,required:!0}},data:()=>({type:null,date:null,reason:null,visible:!1}),methods:{hide:function(){this.type=this.date=this.reason=null,this.visible=!1},create:async function(){if(d.log("submit"),!(await o({userId:this.userId,reason:this.reason,endDate:this.date,type:this.type},{RequestVerificationToken:this.csrf})).ok)return;location.reload()}},template:`
		<div class="my-modal" v-if="visible" @click.self="hide">
			<div class="content">
				<strong>Create infraction</strong>
				<hr>

			  	<form class="form" v-on:submit.prevent="create">
				  <div class="o-form-group">
				    <label for="type">Infraction type</label>
				    <select id="type" v-model="type">
				      <option v-for="t in this.types"
				              :value="t"
				              :key="t">
				        {{t}}
				      </option>
				    </select>
				  </div>

				  <div class="o-form-group">
				    <label for="time">Expiration date</label>
				    <input type="date" id="time" v-model="date">
				  </div>

				  <div class="o-form-group">
				    <label for="reason">Reason</label>
				    <textarea id="reason" v-model="reason"></textarea>
				  </div>

				  <div class="o-form-group">
                    <input type="submit" value="Create">
				  </div>
			    </form>

			</div>
		</div>
	`});

//# debugId=2C239466AA23765464756E2164756E21
