function u(t){return t===Object(t)}function d(t){return JSON.parse(JSON.stringify({...t,__isCopied__:!0}))}var l=(t)=>u(t)?d(t):t,a={log:(t)=>console.log(l(t)),info:(t)=>console.info(l(t)),warn:(t)=>console.warn(l(t)),error:(t)=>console.error(l(t)),debug:(t)=>console.debug(l(t))};var m=[...document.querySelectorAll("input.o-form-control")];for(let t of m)t.dataset.init=t.value,t.addEventListener("input",(i)=>{let n=i.target;if(a.log(n.value!==n.dataset.init),n.value!==n.dataset.init)n.classList.add("changed");else n.classList.remove("changed")});

//# debugId=705580FB67513B4864756E2164756E21
