(async()=>{Vue.config.errorHandler=(e)=>{console.info(e.message)},Vue.config.ignoredElements=[/o-*/],await(async()=>{if("serviceWorker"in navigator)try{let e=await navigator.serviceWorker.register("/js/dist/sw/cache-service-worker.js",{scope:"/"});if(e.installing)console.info("Service worker installing");else if(e.waiting)console.info("Service worker installed");else if(e.active)console.info("Service worker active")}catch(e){console.error(`Registration failed with ${e}`)}else console.warn("Service workers are not supported")})()})();

//# debugId=C95B97B4FE88DA6264756E2164756E21
//# sourceMappingURL=site.js.map
