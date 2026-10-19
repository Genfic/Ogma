import { GetAdminApiUsersSearch } from "@g/paths-internal";
import { $query } from "@h/dom";
import { debounce } from "es-toolkit";

const searchbar = $query<HTMLInputElement>("input#username");
const autocomplete = $query<HTMLDataListElement>("datalist#autocomplete");

const search = debounce(async () => {
	const res = await GetAdminApiUsersSearch(searchbar.value);
	if (!res.ok) {
		console.error(res.statusText);
		return;
	}

	autocomplete.innerHTML = res.data
		.map((d) => `<option value="${d.name}">${d.name} (${(1 - d.distance).toFixed(2)})</option>`)
		.join("\n");
}, 200);

searchbar.addEventListener("input", () => {
	search();
});
