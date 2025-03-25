var d=new DOMParser,l=(t)=>{return d.parseFromString(t,"text/html").body.childNodes[0]};var s;((n)=>{n.bold="bold";n.italic="italic";n.underline="underline";n.spoiler="spoiler";n.link="link"})(s||={});var f=`
		<nav class="button-group toolbar">
		  <button type="button" class="btn" data-action="bold" title="bold">
            <o-icon icon="lucide:bold" class="material-icons-outlined"></o-icon>
		  </button>
		  <button type="button" class="btn" data-action="italic" title="italic">
            <o-icon icon="lucide:italic" class="material-icons-outlined" ></o-icon>
		  </button>
		  <button type="button" class="btn" data-action="underline" title="underline">
            <o-icon icon="lucide:underline" class="material-icons-outlined" ></o-icon>
		  </button>
		  <button type="button" class="btn" data-action="spoiler" title="spoiler">
            <o-icon icon="lucide:eye-closed" class="material-icons-outlined" ></o-icon>
		  </button>
		  <button type="button" class="btn" data-action="link" title="link">
            <o-icon icon="lucide:link" class="material-icons-outlined" ></o-icon>
		  </button>
		</nav>`.split(`
`).map((t)=>t.trim()).join(""),b=new Map([["bold",{prefix:"**",suffix:"**"}],["italic",{prefix:"*",suffix:"*"}],["underline",{prefix:"_",suffix:"_"}],["spoiler",{prefix:"||",suffix:"||"}],["link",{prefix:"[",suffix:"]()"}]]),p=[...document.querySelectorAll("[data-md=true]")];for(let t of p){let o=l(f);for(let e of[...o.querySelectorAll("button.btn[data-action]")]){let a=s[e.dataset.action];e.addEventListener("click",(m)=>{let{prefix:n,suffix:r}=b[a],c=t.selectionStart,i=t.selectionEnd,u=t.value.substring(c,i);t.setRangeText(`${n}${u}${r}`,c,i,"preserve"),t.selectionStart=t.selectionEnd=i+n.length,t.focus()})}t.before(o)}

//# debugId=DB0F357B4E7D142F64756E2164756E21
//# sourceMappingURL=markdown-toolbar.js.map
