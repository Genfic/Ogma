var n=[...document.querySelectorAll("input[id]")];for(let e of n){let s=document.querySelector(`[data-for="${e.id}"]`);if(!s)continue;e.addEventListener("focusin",()=>{s.classList.add("visible")}),e.addEventListener("focusout",()=>{s.classList.remove("visible")})}

//# debugId=61D8D8CF983B095B64756E2164756E21
