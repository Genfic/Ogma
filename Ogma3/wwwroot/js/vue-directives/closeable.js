Vue.directive("closable",{bind(y,z,q){let w=(j)=>{j.stopPropagation();let{handler:A,exclude:B}=z.value,p=!1;for(let D of B)if(!p)p=q.context.$refs[D].contains(j.target);if(!y.contains(j.target)&&!p)q.context[A]()};document.addEventListener("click",w),document.addEventListener("touchstart",w)},unbind(){document.removeEventListener("click",handleOutsideClick),document.removeEventListener("touchstart",handleOutsideClick)}});

//# debugId=42BD550A1B74928964756E2164756E21
