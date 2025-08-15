import type { JSX } from "solid-js";

export function LucideClock(props: JSX.IntrinsicElements["svg"]) {
    return (
        <svg xmlns="http://www.w3.org/2000/svg" width="24px" height="24px" viewBox="0 0 24 24" {...props}>
            <title>lucide clock</title>
            <g fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"><path d="M12 6v6l4 2"/><circle cx="12" cy="12" r="10"/></g>
        </svg>
    );
}
