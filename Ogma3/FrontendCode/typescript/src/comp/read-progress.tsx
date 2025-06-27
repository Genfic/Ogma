import { clamp, normalize } from "@h/math-helpers";
import type { Empty } from "@t/utils";
import { type ComponentType, customElement } from "solid-element";
import { onCleanup, onMount } from "solid-js";
import { Styled } from "./common/_styled";
import css from "./read-progress.css";

const ReadProgress: ComponentType<Empty> = (_, { element }) => {
	let progress = $signal(0);
	let ticking = $signal(false);
	let read = $signal(false);

	const updateProgress = () => {
		const parent = element.parentElement;
		const elBottom = parent.getBoundingClientRect().bottom;
		const percent = elBottom - window.innerHeight;
		const containerHeight = parent.offsetTop + parent.offsetHeight;
		const maxHeight = Math.max(containerHeight - window.innerHeight, 0);

		const newProgress = 1 - clamp(normalize(percent, 0, maxHeight));
		progress = newProgress;

		if (newProgress >= 1 && !read) {
			read = true;
			element.dispatchEvent(new CustomEvent("read"));
		}
	};

	const handleScroll = () => {
		if (!ticking) {
			window.requestAnimationFrame(() => {
				updateProgress();
				ticking = false;
			});
			ticking = true;
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

	return <div class="bar" style={{ width: `${progress * 100}%` }} />;
};

customElement("o-read-progress", {}, Styled(ReadProgress, css));
