import{f as i}from"./chunk-rtz3801n.js";import{i as l,x as n}from"./chunk-gma6eqe0.js";import"./chunk-qtq4kp2t.js";import{$ as k,F as r,S as c,T as m,Z as a,aa as f}from"./chunk-59rmnzyw.js";var u=c("<button type=button>"),B=(o)=>{k();let[s,d]=r(o.isBlocked),p=async()=>{let t=await(s()?l:n)({name:o.userName},{RequestVerificationToken:o.csrf});if(t.ok)d(t.data);else i.warn(t.statusText)};return(()=>{var e=u();return e.$$click=p,a(e,()=>s()?"Unblock":"Block"),e})()};f("o-block",{userName:"",csrf:"",isBlocked:!1},B);m(["click"]);

//# debugId=19D66CB6F868EB2464756E2164756E21
