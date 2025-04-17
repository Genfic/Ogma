import{b as i}from"./chunk-118zdqkz.js";import{e as l,t as n}from"./chunk-swmx46mn.js";import{G as c,H as m,M as a,N as k,O as f,w as r}from"./chunk-1dj32eyg.js";var u=c("<button type=button>"),B=(o)=>{k();let[s,d]=r(o.isBlocked),p=async()=>{let t=await(s()?l:n)({name:o.userName},{RequestVerificationToken:o.csrf});if(t.ok)d(t.data);else i.warn(t.statusText)};return(()=>{var e=u();return e.$$click=p,a(e,()=>s()?"Unblock":"Block"),e})()};f("o-block",{userName:"",csrf:"",isBlocked:!1},B);m(["click"]);

//# debugId=F523AE1B103ACCBD64756E2164756E21
