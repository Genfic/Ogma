var c=(d)=>new RegExp(`(^|;)\\s*${d}\\s*=\\s*([^;]+)`,"i"),n=(d)=>{return document.cookie.match(c(d))?.at(2)};var u="visible",s=[...document.querySelectorAll("input[id]")];for(let d of s){let t=document.querySelector(`[data-for="${d.id}"]`);if(!t)continue;d.addEventListener("focusin",()=>{t.classList.add(u)}),d.addEventListener("focusout",()=>{t.classList.remove(u)})}var f=document.querySelector(".cf-turnstile"),l=n("theme");f.dataset.theme=l;

//# debugId=5D7009036B839A3564756E2164756E21
