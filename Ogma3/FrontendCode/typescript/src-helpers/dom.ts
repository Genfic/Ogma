const parser = new DOMParser();

export const parseDom = (html: string): HTMLElement => {
	return parser.parseFromString(html, "text/html").body.firstElementChild as HTMLElement;
};

function $query<TElement extends HTMLElement>(selector: string): TElement;
function $query<TElement extends HTMLElement>(selector: string, noThrow: false): TElement;
function $query<TElement extends HTMLElement>(selector: string, noThrow: true): TElement | null;
function $query<TElement extends HTMLElement>(selector: string, noThrow?: boolean): TElement | null {
	const el = document.querySelector(selector);
	if (noThrow) {
		return el as TElement | null;
	}
	if (!el) {
		throw new Error(`Element not found: ${selector}`);
	}
	return el as TElement;
}

function $queryAll<TElement extends HTMLElement>(selector: string): TElement[] {
	return [...document.querySelectorAll(selector)] as TElement[];
}

function $id<TElement extends HTMLElement>(id: string): TElement;
function $id<TElement extends HTMLElement>(id: string, noThrow: false): TElement;
function $id<TElement extends HTMLElement>(id: string, noThrow: true): TElement | null;
function $id<TElement extends HTMLElement>(id: string, noThrow?: boolean): TElement | null {
	const el = document.getElementById(id);
	if (noThrow) {
		return el as TElement | null;
	}
	if (!el) {
		throw new Error(`Element not found: ${id}`);
	}
	return el as TElement;
}

export { $query, $queryAll, $id };

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
