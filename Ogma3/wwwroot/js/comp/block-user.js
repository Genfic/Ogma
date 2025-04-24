import{c as i}from"./chunk-sx0ed2ft.js";import{f as l,u as n}from"./chunk-yqjz5kwv.js";import{K as c,L as m,R as a,S as k,T as f,x as r}from"./chunk-a53ywpea.js";var u=c("<button type=button>"),B=(o)=>{k();let[s,d]=r(o.isBlocked),p=async()=>{let t=await(s()?l:n)({name:o.userName},{RequestVerificationToken:o.csrf});if(t.ok)d(t.data);else i.warn(t.statusText)};return(()=>{var e=u();return e.$$click=p,a(e,()=>s()?"Unblock":"Block"),e})()};f("o-block",{userName:"",csrf:"",isBlocked:!1},B);m(["click"]);

//# debugId=4634F5C21E11354E64756E2164756E21
