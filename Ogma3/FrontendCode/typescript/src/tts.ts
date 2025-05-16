import { $id, $query } from "@h/dom";

(() => {
	const button = $id("tts-btn");
	const stopBtn = $id("tts-stop");
	const icon = $id("tts-btn-ico");
	const body = $query(".tts-body");
	const utterance = new SpeechSynthesisUtterance(body.innerText.trim());

	const icons = {
		play: "play_arrow",
		pause: "pause",
	} as const;

	const synth = window.speechSynthesis;

	button.addEventListener("click", () => {
		if (!("speechSynthesis" in window)) {
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

	stopBtn.addEventListener("click", () => {
		if (synth.speaking) {
			synth.cancel();
			icon.innerText = icons.play;
		}
	});
})();
