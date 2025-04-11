var n=0,t=!1,s=document.getElementById("top-nav"),o=document.getElementById("burger"),l=0;function i(e){s.classList.toggle("compact",e-l>0),l=e}window.addEventListener("scroll",()=>{if(n=window.scrollY,!t)window.requestAnimationFrame(()=>{i(n),t=!1}),t=!0});o?.addEventListener("click",()=>{s.classList.toggle("visible")});

//# debugId=A3E1AA95621E283E64756E2164756E21
