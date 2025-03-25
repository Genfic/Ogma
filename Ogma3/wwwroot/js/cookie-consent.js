function u(n,t,s=null,y=!1,D=null,b=null){let o=`${n}=${t}`;if(s)o+=`; expires=${s.toUTCString()}`;if(y)o+=`; secure=${String(y)}`;if(D)o+=`; samesite=${D}`;if(b)o+=`; path=${b}`;document.cookie=o}var r=(n,t={})=>{return new Date(n.getFullYear()+(t.years??0),n.getMonth()+(t.months??0),n.getDate()+(t.days??0),n.getHours()+(t.hours??0),n.getMinutes()+(t.minutes??0),n.getSeconds()+(t.seconds??0),n.getMilliseconds()+(t.milliseconds??0))};var c=document.getElementById("cookie-consent"),d=c.querySelector("button#cookie-consent-button");d.addEventListener("click",()=>{let n=r(new Date,{years:100});u("cookie-consent","true",n,!0,"strict","/"),c.style.display="none"});

//# debugId=1A555611E1F02E4264756E2164756E21
//# sourceMappingURL=cookie-consent.js.map
