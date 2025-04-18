import { addStyle } from "@h/jsx-wc-style";
import { clamp, normalize } from "@h/math-helpers";
import { type ComponentType, customElement } from "solid-element";
import { createSignal, onCleanup, onMount } from "solid-js";
import { minifyCss } from "@h/minify.macro" with { type: "macro" };

const ReadProgress: ComponentType<null> = (_, { element }) => {
	const [progress, setProgress] = createSignal(0);
	const [ticking, setTicking] = createSignal(false);
	const [read, setRead] = createSignal(false);

	const updateProgress = () => {
		const parent = element.parentElement;
		const elBottom = parent.getBoundingClientRect().bottom;
		const percent = elBottom - window.innerHeight;
		const containerHeight = parent.offsetTop + parent.offsetHeight;
		const maxHeight = Math.max(containerHeight - window.innerHeight, 0);

		const newProgress = 1 - clamp(normalize(percent, 0, maxHeight));
		setProgress(newProgress);

		if (newProgress >= 1 && !read()) {
			setRead(true);
			element.dispatchEvent(new CustomEvent("read"));
		}
	};

	const handleScroll = () => {
		if (!ticking()) {
			window.requestAnimationFrame(() => {
				updateProgress();
				setTicking(false);
			});
			setTicking(true);
		}
	};

	const handleResize = () => updateProgress();

	onMount(() => {
		// language=CSS
		addStyle(
			element,
			minifyCss(`
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
			`),
		);

		document.addEventListener("scroll", handleScroll);
		window.addEventListener("resize", handleResize);
		updateProgress();
	});

	onCleanup(() => {
		document.removeEventListener("scroll", handleScroll);
		window.removeEventListener("resize", handleResize);
	});

	return <div class="bar" style={{ width: `${progress() * 100}%` }} />;
};

customElement("o-read-progress", null, ReadProgress);
