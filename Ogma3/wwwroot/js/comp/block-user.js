import{c as i}from"./chunk-sx0ed2ft.js";import{f as l,u as n}from"./chunk-yqjz5kwv.js";import{x as k,y as f}from"./chunk-ap729v4f.js";import{A as r,N as c,O as m,U as a}from"./chunk-y7y724mm.js";var u=c("<button type=button>"),B=(o)=>{k();let[s,d]=r(o.isBlocked),p=async()=>{let t=await(s()?l:n)({name:o.userName},{RequestVerificationToken:o.csrf});if(t.ok)d(t.data);else i.warn(t.statusText)};return(()=>{var e=u();return e.$$click=p,a(e,()=>s()?"Unblock":"Block"),e})()};f("o-block",{userName:"",csrf:"",isBlocked:!1},B);m(["click"]);

//# debugId=FFEF1292905A442164756E2164756E21
