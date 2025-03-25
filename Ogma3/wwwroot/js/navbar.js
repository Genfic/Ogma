var n=0,t=!1,o=document.getElementById("top-nav"),s=document.getElementById("burger"),l=0;function i(e){o.classList.toggle("compact",e-l>0),l=e}window.addEventListener("scroll",()=>{if(n=window.scrollY,!t)window.requestAnimationFrame(()=>{i(n),t=!1}),t=!0});s?.addEventListener("click",()=>{o.classList.toggle("visible")});

//# debugId=E52FAA0479ED62EC64756E2164756E21
//# sourceMappingURL=navbar.js.map
