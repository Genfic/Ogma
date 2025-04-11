Vue.config.errorHandler=(e)=>{console.info(e.message)};Vue.config.ignoredElements=[/o-*/];var r=async()=>{if("serviceWorker"in navigator)try{let e=await navigator.serviceWorker.register("/js/sw/cache-service-worker.js",{scope:"/"});if(e.installing)console.info("Service worker installing");else if(e.waiting)console.info("Service worker installed");else if(e.active)console.info("Service worker active")}catch(e){console.error(`Registration failed with ${e}`)}else console.warn("Service workers are not supported")};await r();

//# debugId=D3E326E1ABC21AFC64756E2164756E21
