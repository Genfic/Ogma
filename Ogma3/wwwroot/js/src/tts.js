`use strict`;
(function () {
    const button = document.getElementById('tts-btn');
    const stopBtn = document.getElementById('tts-stop');
    const icon = document.getElementById('tts-btn-ico');
    const body = document.querySelector('.tts-body');
    const utterance = new SpeechSynthesisUtterance(body.innerText.trim());

    const icons = Object.freeze({
        play: 'play_arrow',
        pause: 'pause',
    });

    const synth = window.speechSynthesis;

    button.addEventListener('click', function () {
        if (!('speechSynthesis' in window)) {
            alert("Your browser doesn't support text-to-speech!");
            return;
        }

        if (synth.paused) {
            synth.resume();
            icon.innerText = icons.pause;
        } else if (synth.speaking) {
            synth.pause();
            icon.innerText = icons.play;
        } else {
            synth.speak(utterance);
            icon.innerText = icons.pause;
        }
    });

    stopBtn.addEventListener("click", function () {
        if (synth.speaking) {
            synth.cancel();
            icon.innerText = icons.play;
        }
    });
}());