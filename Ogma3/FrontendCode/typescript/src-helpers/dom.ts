const parser = new DOMParser();

export const parseDom = (html: string): HTMLElement => {
	return parser.parseFromString(html, "text/html").body.firstElementChild as HTMLElement;
};

/**
 * Finds the first following sibling element matching a CSS selector.
 * Searches forward only from the starting element.
 *
 * @param el The starting element.
 * @param selector The CSS selector to match siblings against.
 * @returns The first matching following sibling, or null if none is found.
 */
export const findNextSibling = (el: Element, selector: string): Element | null => {
	if (!el) return null; // Handle cases where the starting element doesn't exist

	let currentSibling = el.nextElementSibling; // Start with the immediate next sibling

	// Loop through all following siblings
	while (currentSibling) {
		// Check if the current sibling matches the selector
		if (currentSibling.matches(selector)) {
			// Found the matching sibling
			return currentSibling;
		}
		// Move to the next sibling
		currentSibling = currentSibling.nextElementSibling;
	}

	// No matching sibling found after the starting element
	return null;
};
