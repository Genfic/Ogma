import type { JSX } from "solid-js";

export function LucideStrikethrough(props: JSX.IntrinsicElements["svg"]) {
    return (
        <svg xmlns="http://www.w3.org/2000/svg" width="24px" height="24px" viewBox="0 0 24 24" {...props}>
            <title>lucide strikethrough</title>
            <path fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 4H9a3 3 0 0 0-2.83 4M14 12a4 4 0 0 1 0 8H6m-2-8h16"/>
        </svg>
    );
}
