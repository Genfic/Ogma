var r="GET",T="POST";var c="DELETE";async function h(o,n,e,s,a){let t=await fetch(o,{method:n,headers:{"content-type":"application/json",...s},body:e&&JSON.stringify(e),...a}),d=t.headers.get("content-type")?.includes("application/json")?await t.json():await t.text();return{ok:t.ok,status:t.status,statusText:t.statusText,headers:t.headers,data:d}}
export{r as D,T as E,c as F,h as G};

//# debugId=3A74757DD0E3B35664756E2164756E21
