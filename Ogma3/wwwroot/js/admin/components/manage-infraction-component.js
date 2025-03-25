async function r(t,i,a,o,u){let n=await fetch(t,{method:i,headers:{"Content-Type":"application/json",...o},body:a?JSON.stringify(a):null,...u}),m=n.headers.get("content-type"),s;if(m?.includes("application/json"))s=await n.json();else s=await n.text();return{ok:n.ok,status:n.status,statusText:n.statusText,headers:n.headers,data:s}}var d=async(t,i,a)=>await r("/admin/api/infractions","POST",t,i,a);function p(t){return t===Object(t)}function I(t){return JSON.parse(JSON.stringify({...t,__isCopied__:!0}))}var e=(t)=>p(t)?I(t):t,c={log:(t)=>console.log(e(t)),info:(t)=>console.info(e(t)),warn:(t)=>console.warn(e(t)),error:(t)=>console.error(e(t)),debug:(t)=>console.debug(e(t))};Vue.component("manage-infraction",{props:{csrf:{type:String,required:!0},userId:{type:Number,required:!0},types:{type:Array,required:!0}},data:()=>({type:null,date:null,reason:null,visible:!1}),methods:{hide:function(){this.type=this.date=this.reason=null,this.visible=!1},create:async function(){if(c.log("submit"),!(await d({userId:this.userId,reason:this.reason,endDate:this.date,type:this.type},{RequestVerificationToken:this.csrf})).ok)return;location.reload()}},template:`
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

//# debugId=0EA65452CA63EC3D64756E2164756E21
//# sourceMappingURL=manage-infraction-component.js.map
