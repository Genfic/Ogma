var r="GET",T="POST";var c="DELETE";async function h(o,n,e,s,a){let t=await fetch(o,{method:n,headers:{"content-type":"application/json",...s},body:e&&JSON.stringify(e),...a}),d=t.headers.get("content-type")?.includes("application/json")?await t.json():await t.text();return{ok:t.ok,status:t.status,statusText:t.statusText,headers:t.headers,data:d}}
export{r as C,T as D,c as E,h as F};

//# debugId=21F014A4624D10C264756E2164756E21
