import { Styled } from "./common/_styled";
import { clamp, normalize } from "@h/math-helpers";
import { type ComponentType, customElement } from "solid-element";
import { createSignal, onCleanup, onMount } from "solid-js";
import css from "./read-progress.css";
import type { Empty } from "@t/utils";

const ReadProgress: ComponentType<Empty> = (_, { element }) => {
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

customElement("o-read-progress", {}, Styled(ReadProgress, css));
