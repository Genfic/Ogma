async function i(t,a,n,r,d){let e=await fetch(t,{method:a,headers:{"Content-Type":"application/json",...r},body:n?JSON.stringify(n):null,...d}),p=e.headers.get("content-type"),s;if(p?.includes("application/json"))s=await e.json();else s=await e.text();return{ok:e.ok,status:e.status,statusText:e.statusText,headers:e.headers,data:s}}var o=async(t,a,n)=>await i("/api/reports","POST",t,a,n);Vue.component("vue-report-modal",{props:{csrf:{type:String,required:!0},itemType:{type:String,required:!0},itemId:{type:Number,required:!0}},data:function(){return{visible:!1,reason:"",message:null,btnClass:"",mutId:this.itemId}},methods:{hide:function(){this.reason="",this.visible=!1,this.$refs.text.clear()},send:async function(){if(!this.$refs.text.validate)return;if((await o({itemId:this.mutId,reason:this.reason,itemType:this.itemType},{RequestVerificationToken:this.csrf})).ok)this.message="Report delivered!",this.btnClass="green";else this.message="An error has occurred.",this.btnClass="red"}},template:`
		<div class="report-modal my-modal" v-if="visible" @click.self="hide" v-cloak>
		<div class="content">

			<div class="header">
				<span>Report content</span>
			</div>

			<form class="form">
				<textarea-counter ref="text"
								  label="Reason"
								  desc="Why do you want to report this content?"
								  validate-msg="The {0} must be between {2} and {1} characters"
								  :min="30" :max="500"
								  v-model="reason">
				</textarea-counter>
			</form>

			<br>

			<button class="btn" :class="btnClass" v-on:click="send">
				{{ message ?? 'Send report' }}
			</button>

		</div>
		</div>
	`});

//# debugId=3363A7C7E53BDF0164756E2164756E21
