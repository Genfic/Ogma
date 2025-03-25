(function(){let s=async(t)=>{let n=await caches.keys();if(n.includes(t))return;console.info("New version of JS manifest found, purging SW cache."),await Promise.all(n.map((e)=>caches.delete(e))),console.info("SW cache purged.")};this.addEventListener("activate",async(t)=>{let e=await(await fetch("/manifest.js.json")).json();await s(e.GeneratedAt);let i=Object.entries(e.Files).map(([a,c])=>`${a}?v=${c}`);t.waitUntil(caches.open(e.GeneratedAt).then((a)=>a.addAll(i)))})}).call(self);

//# debugId=24D805FD38623E5264756E2164756E21
//# sourceMappingURL=cache-service-worker.js.map
