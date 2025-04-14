import { clamp, normalize } from "@h/math-helpers";
import { customElement } from "solid-element";
import { createSignal, onCleanup, onMount } from "solid-js";

customElement("o-read-progress", {}, (props) => {
	const [progress, setProgress] = createSignal(0);
	const [ticking, setTicking] = createSignal(false);
	const [read, setRead] = createSignal(false);

	onMount(() => {
		// Add styles to shadow root
		const style = document.createElement("style");
		style.textContent = `
      :host {
        position: sticky;
        inset: auto 0 0;
      }

      .bar {
        position: relative;
        height: 3px;
        background-color: var(--accent);
        transition: width 50ms ease-out;
      }
    `;
		props.element.shadowRoot.appendChild(style);

		const handleScroll = () => {
			if (!ticking()) {
				window.requestAnimationFrame(() => {
					updateProgress();
					setTicking(false);
				});
				setTicking(true);
			}
		};

		const updateProgress = () => {
			const parent = props.element.parentElement;
			const elBottom = parent.getBoundingClientRect().bottom;
			const percent = elBottom - window.innerHeight;
			const containerHeight = parent.offsetTop + parent.offsetHeight;
			const maxHeight = Math.max(containerHeight - window.innerHeight, 0);

			const newProgress = 1 - clamp(normalize(percent, 0, maxHeight));
			setProgress(newProgress);

			if (newProgress >= 1 && !read()) {
				setRead(true);
				props.element.dispatchEvent(new CustomEvent("read"));
			}
		};

		const handleResize = () => updateProgress();

		document.addEventListener("scroll", handleScroll);
		window.addEventListener("resize", handleResize);
		updateProgress();

		onCleanup(() => {
			document.removeEventListener("scroll", handleScroll);
			window.removeEventListener("resize", handleResize);
		});
	});

	return () => <div class="bar" style={{ width: `${progress() * 100}%` }}></div>;
});
