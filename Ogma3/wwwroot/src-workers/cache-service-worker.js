var n=async()=>{let t=(await(await fetch("manifest.txt")).text()).split(`
`);return{stamp:`precache-${new Date(t[0]).getTime()}`,files:t.slice(1).map((a)=>{let[o,i]=a.split(":");return`${o}?v=${i??""}`})}},l=async()=>{let{stamp:e,files:c}=await n(),t=await caches.open(e);await Promise.all(c.map(async(s)=>{try{let a=await fetch(s,{cache:"reload"});if(!a.ok)console.warn("Failed to fetch for precache:",s);await t.put(s,a)}catch(a){console.error("Error precaching",s,a)}})),console.info(`Precached ${c.length} assets into cache ${e}`)},r=async()=>{let{stamp:e}=await n(),c=await caches.keys();await Promise.all(c.filter((t)=>t!==e).map((t)=>caches.delete(t))),await self.clients.claim(),console.info("Cleared old caches. Active cache:",e)};self.addEventListener("install",(e)=>{e.waitUntil(l())});self.addEventListener("activate",(e)=>{e.waitUntil(r())});

//# debugId=ABAA88396EF26AC864756E2164756E21
//# sourceMappingURL=cache-service-worker.js.map
