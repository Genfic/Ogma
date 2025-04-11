var l=new Vue({el:"#search",data:{els:{query:null,rating:null,sort:null,tags:null},dis:{query:!1,rating:!1,sort:!1,tags:!1}},methods:{submit:function(e){this.els.tags=this.$refs.tags.selected.length>0;for(let[t,s]of Object.entries(this.els))this.dis[t]=!s;this.dis.tags=this.$refs.tags.checkDisabled(),this.$nextTick(()=>e.target.submit())}},mounted(){this.els={query:document.getElementById("query").value,rating:document.getElementById("rating").value,sort:document.getElementById("sort").value,tags:[]}}});

//# debugId=63FF390B8851775D64756E2164756E21
