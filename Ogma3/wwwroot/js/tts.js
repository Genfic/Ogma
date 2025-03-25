(()=>{let s=document.getElementById("tts-btn"),c=document.getElementById("tts-stop"),t=document.getElementById("tts-btn-ico"),o=document.querySelector(".tts-body"),i=new SpeechSynthesisUtterance(o.innerText.trim()),n=Object.freeze({play:"play_arrow",pause:"pause"}),e=window.speechSynthesis;s.addEventListener("click",()=>{if(!("speechSynthesis"in window)){alert("Your browser doesn't support text-to-speech!");return}if(e.paused)e.resume(),t.innerText=n.pause;else if(e.speaking)e.pause(),t.innerText=n.play;else e.speak(i),t.innerText=n.pause}),c.addEventListener("click",()=>{if(e.speaking)e.cancel(),t.innerText=n.play})})();

//# debugId=7F91E8380827B63864756E2164756E21
//# sourceMappingURL=tts.js.map
