import { $query } from "@h/dom";

const timezone = Intl.DateTimeFormat().resolvedOptions().timeZone;

const input = $query<HTMLInputElement>("input#timezone");

console.log("Timezone", timezone, "input", input);

input.value = timezone;
input.setAttribute("value", timezone);
