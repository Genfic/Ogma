import{h as i}from"./chunk-gchvq6pe.js";import{k as l,z as n}from"./chunk-1pgp53hp.js";import"./chunk-v3754c6r.js";import{$ as k,G as r,T as c,U as m,_ as a,aa as f}from"./chunk-3v8fzpj7.js";var u=c("<button type=button>"),B=(o)=>{k();let[s,d]=r(o.isBlocked),p=async()=>{let t=await(s()?l:n)({name:o.userName},{RequestVerificationToken:o.csrf});if(t.ok)d(t.data);else i.warn(t.statusText)};return(()=>{var e=u();return e.$$click=p,a(e,()=>s()?"Unblock":"Block"),e})()};f("o-block",{userName:"",csrf:"",isBlocked:!1},B);m(["click"]);

//# debugId=FE47515C402C5A9E64756E2164756E21
