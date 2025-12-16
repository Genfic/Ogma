import { $query } from "@h/dom";
import { render } from "solid-js/web";
import { LucideTrash2 } from "./icons/LucideTrash2";

const table = $query("table.credits-table > tbody");
const addBtn = $query<HTMLButtonElement>("button.add-row");

const reindexRows = () => {
	const rows = table.querySelectorAll("tr");
	for (let i = 0; i < rows.length; i++) {
		const inputs = rows[i].querySelectorAll("input");
		for (const input of inputs) {
			const name = input.getAttribute("name");
			if (name) {
				const newName = name.replace(/Input\.Credits\[\d+\]/, `Input.Credits[${i}]`);
				input.setAttribute("name", newName);
			}
		}
	}
	index = rows.length;
};

const newRow = (idx: number, role = "", name = "", link = "") => {
	const row = document.createElement("tr");
	render(
		() => (
			<>
				<td>
					<input list="credit-roles" type="text" name={`Input.Credits[${idx}].Role`} value={role} />
				</td>
				<td>
					<input type="text" name={`Input.Credits[${idx}].Name`} value={name} />
				</td>
				<td>
					<input type="text" name={`Input.Credits[${idx}].Link`} value={link} />
				</td>
				<td>
					<button
						type="button"
						class="btn remove"
						onclick={() => {
							row.remove();
							reindexRows();
						}}
					>
						<LucideTrash2 />
					</button>
				</td>
			</>
		),
		row,
	);
	return row;
};

let index = 0;

for (const row of [...table.querySelectorAll("tr")]) {
	const entries = new Map(
		(["role", "name", "link"] as const).map((prop) => [
			prop,
			(row.querySelector(`input[name$=".${prop}" i]`) as HTMLInputElement).value,
		]),
	);
	table.appendChild(newRow(index++, entries.get("role"), entries.get("name"), entries.get("link")));
	row.remove();
}

addBtn.addEventListener("click", () => {
	table.appendChild(newRow(index++));
});
