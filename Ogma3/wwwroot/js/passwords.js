var r=[...document.querySelectorAll("input[type=password]")];for(let e of r)e.nextElementSibling?.querySelector(".show-password").addEventListener("click",(t)=>{if(t.preventDefault(),e.type==="password")e.type="text",t.currentTarget.querySelector("use").setAttribute("href","/svg/spritesheet.svg#lucide:eye");else e.type="password",t.currentTarget.querySelector("use").setAttribute("href","/svg/spritesheet.svg#lucide:eye-closed")});

//# debugId=69AA22056B1BD5F064756E2164756E21
