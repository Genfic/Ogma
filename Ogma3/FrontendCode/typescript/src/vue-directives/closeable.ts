import type Vue from "vue";
import type { DirectiveOptions, VNodeDirective, VNode } from "vue";

// 1. Define the expected type for the directive's value (v-closable="...")
interface ClosableBindingValue {
	handler: string; // The name of the method on the component instance
	exclude: string[]; // Array of $ref names to exclude from closing
}

// 2. Augment VNode to include context with $refs and dynamic method access
// In Vue 2, vnode.context is the component instance.
// $refs can contain HTMLElement, Vue instances, or arrays of them, or undefined.
// We also need to call a method by string name, so we allow string indexing.
interface VNodeWithContext extends VNode {
	context: Vue & {
		$refs: { [key: string]: HTMLElement | Vue | HTMLElement[] | Vue[] | undefined };
		[key: string]: (() => void) | unknown; // Allow accessing methods by string name
	};
}

// 3. Use a Symbol to attach the handler safely to the element (prevents naming collisions)
const outsideClickHandlerSymbol = Symbol("outsideClickHandler");

// 4. Define the directive options object with TypeScript types
const ClosableDirective: DirectiveOptions = {
	// The `bind` hook is called once when the directive is first bound to the element.
	bind(el: HTMLElement, binding: VNodeDirective, vnode: VNodeWithContext) {
		// Ensure the binding value has the expected structure
		if (typeof binding.value !== "object" || !binding.value.handler || !Array.isArray(binding.value.exclude)) {
			console.error('v-closable directive requires an object with "handler" (string) and "exclude" (string[])');
			return; // Exit if value is not correct
		}

		const { handler, exclude } = binding.value as ClosableBindingValue;

		// Check if the handler method exists on the component context
		if (typeof vnode.context[handler] !== "function") {
			console.error(
				`v-closable directive: Handler method "${handler}" not found on component instance.`,
				vnode.context,
			);
			return; // Exit if handler doesn't exist
		}

		// The click/touchstart handler function
		const handleOutsideClick = (e: MouseEvent | TouchEvent) => {
			// Stop propagation immediately on click inside the directive's element
			// This prevents clicks inside the element from triggering the document listener
			if (el.contains(e.target as Node)) {
				e.stopPropagation(); // Important: Ensure click inside doesn't bubble up to document handler
				return; // Do nothing if clicked inside the bound element
			}

			// If propagation wasn't stopped above, it was an outside click.
			// Now check if the clicked element is one of the excluded ones.

			let clickedOnExcludedEl = false;

			// Ensure event target is a Node for 'contains' check
			const targetNode = e.target as Node;

			if (exclude && exclude.length > 0) {
				for (const refName of exclude) {
					// Avoid unnecessary checks once an excluded element is found
					if (clickedOnExcludedEl) {
						break;
					}

					// Get the excluded element using the reference name from the component's $refs
					const excludedElRef = vnode.context.$refs[refName];

					if (excludedElRef) {
						// $refs can return Vue instances, HTMLElement, or arrays.
						// We need the root DOM element for 'contains'.
						// If it's a Vue instance, get its root element $el.
						// If it's an array (e.g., v-for), check if any element in the array contains the target.
						const elementsToCheck: (HTMLElement | undefined)[] = [];

						if (Array.isArray(excludedElRef)) {
							elementsToCheck.push(
								...[...excludedElRef].map((item) => (item as any).$el || (item as HTMLElement)),
							);
						} else {
							elementsToCheck.push((excludedElRef as any).$el || (excludedElRef as HTMLElement));
						}

						for (const excludedEl of elementsToCheck) {
							// See if this excluded element contains the clicked target
							if (excludedEl?.contains(targetNode)) {
								clickedOnExcludedEl = true;
								break; // Found an excluded element, no need to check further
							}
						}
					}
				}
			}

			// If the clicked element is outside the bound element (`el`)
			// AND not clicked on any of the excluded elements...
			if (!clickedOnExcludedEl) {
				// Then call the handler method on the component context
				const h = vnode.context[handler];
				h instanceof Function && h.call(vnode.context);
			}
		};

		// Store the handler function instance on the element itself
		(el as any)[outsideClickHandlerSymbol] = handleOutsideClick;

		// Register click/touchstart event listeners on the whole page
		document.addEventListener("click", handleOutsideClick);
		document.addEventListener("touchstart", handleOutsideClick); // Consider adding { passive: true } for touch events for performance
	},

	// The `unbind` hook is called once when the directive is unbound from the element.
	unbind(el: HTMLElement, binding: VNodeDirective, vnode: VNodeWithContext) {
		// Retrieve the stored handler function instance from the element
		const handleOutsideClick = (el as any)[outsideClickHandlerSymbol] as (e: MouseEvent | TouchEvent) => void;

		// If the handler was stored (meaning bind successfully ran), remove the listeners
		if (handleOutsideClick) {
			document.removeEventListener("click", handleOutsideClick);
			document.removeEventListener("touchstart", handleOutsideClick);
			// Clean up the stored handler from the element
			delete (el as any)[outsideClickHandlerSymbol];
		}
	},
};

// Register the directive
// (Assuming Vue is available globally or imported elsewhere and used)
// Vue.directive("closable", ClosableDirective); // Uncomment this line to register
