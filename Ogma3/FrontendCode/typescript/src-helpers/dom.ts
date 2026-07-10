const parser = new DOMParser();

export const parseDom = (html: string): HTMLElement => {
	const child = parser.parseFromString(html, "text/html").body.firstElementChild;
	if (child instanceof HTMLElement) {
		return child;
	}
	throw new Error("Invalid HTML");
};

function $query<TElement extends HTMLElement>(selector: string): TElement;
function $query<TElement extends HTMLElement>(selector: string, noThrow: false): TElement;
function $query<TElement extends HTMLElement>(selector: string, noThrow: true): TElement | null;
function $query<TElement extends HTMLElement>(selector: string, noThrow?: boolean): TElement | null {
	const el = document.querySelector<TElement>(selector);
	if (noThrow) {
		return el;
	}
	if (!el) {
		throw new Error(`Element not found: ${selector}`);
	}
	return el;
}

function $queryAll<TElement extends HTMLElement>(selector: string): TElement[] {
	return [...document.querySelectorAll<TElement>(selector)] as TElement[];
}

function $id<TElement extends HTMLElement>(id: string): TElement;
function $id<TElement extends HTMLElement>(id: string, noThrow: false): TElement;
function $id<TElement extends HTMLElement>(id: string, noThrow: true): TElement | null;
function $id<TElement extends HTMLElement>(id: string, noThrow?: boolean): TElement | null {
	const el = document.getElementById(id);

	if (noThrow) {
		return el as unknown as TElement | null;
	}
	if (!el) {
		throw new Error(`Element not found: ${id}`);
	}
	return el as unknown as TElement;
}

function $target<TElement extends HTMLElement>(event: Event): TElement;
function $target<TElement extends HTMLElement>(event: Event, noThrow: false): TElement;
function $target<TElement extends HTMLElement>(event: Event, noThrow: true): TElement | null;
function $target<TElement extends HTMLElement>(event: Event, noThrow?: boolean): TElement | null {
	const t = event.target as HTMLElement;

	if (noThrow) {
		return t as unknown as TElement | null;
	}
	if (!t) {
		throw new Error();
	}
	return t as unknown as TElement;
}

export { $id, $query, $queryAll, $target };

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
