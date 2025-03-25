function d(t){return t===Object(t)}function m(t){return JSON.parse(JSON.stringify({...t,__isCopied__:!0}))}var l=(t)=>d(t)?m(t):t,i={log:(t)=>console.log(l(t)),info:(t)=>console.info(l(t)),warn:(t)=>console.warn(l(t)),error:(t)=>console.error(l(t)),debug:(t)=>console.debug(l(t))};(()=>{let t=[...document.querySelectorAll("input.o-form-control")];for(let a of t)a.dataset.init=a.value,a.addEventListener("input",(u)=>{let n=u.target;if(i.log(n.value!==n.dataset.init),n.value!==n.dataset.init)n.classList.add("changed");else n.classList.remove("changed")})})();

//# debugId=7A3D523DA8E41B9264756E2164756E21
//# sourceMappingURL=settings.js.map
