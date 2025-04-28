import{e as i}from"./chunk-5k1k13kq.js";import{h as l,w as n}from"./chunk-k4ry6v6w.js";import"./chunk-s8q2er36.js";import{E as r,R as c,S as m,Y as a,Z as k,_ as f}from"./chunk-qcet3yp8.js";var u=c("<button type=button>"),B=(o)=>{k();let[s,d]=r(o.isBlocked),p=async()=>{let t=await(s()?l:n)({name:o.userName},{RequestVerificationToken:o.csrf});if(t.ok)d(t.data);else i.warn(t.statusText)};return(()=>{var e=u();return e.$$click=p,a(e,()=>s()?"Unblock":"Block"),e})()};f("o-block",{userName:"",csrf:"",isBlocked:!1},B);m(["click"]);

//# debugId=041A1857DDCC3E9764756E2164756E21
