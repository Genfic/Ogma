export const clickOutside = (element: HTMLElement, callback: () => void) =>
	document.addEventListener("click", (e) => {
		if (!e.composedPath().includes(element)) {
			callback();
		}
	});