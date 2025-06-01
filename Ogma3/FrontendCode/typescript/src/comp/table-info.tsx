import { GetAdminApiTelemetryGetTableInfo } from "@g/paths-internal";
import { convert } from "convert";
import { orderBy } from "es-toolkit";
import { customElement } from "solid-element";
import { type Component, createResource, createSignal, For } from "solid-js";
import { Styled } from "./common/_styled";
import css from "./table-info.css";

const TableInfo: Component = () => {
	const [sortBy, setSortBy] = createSignal<"size" | "name">("size");
	const [sortOrder, setSortOrder] = createSignal<"asc" | "desc">("desc");

	const [tableInfo, { mutate: setTableInfo }] = createResource(
		async () => {
			const res = await GetAdminApiTelemetryGetTableInfo();
			if (!res.ok) {
				throw new Error(res.error);
			}

			const data = res.data;

			const elements = Object.entries(data).map(([k, v]) => ({
				name: k,
				size: Number.parseInt(v),
			}));

			return orderBy(elements, [sortBy()], [sortOrder()]);
		},
		{ initialValue: [] },
	);

	const sort = (by: "size" | "name") => {
		setSortBy(by);
		setSortOrder((prev) => (prev === "desc" ? "asc" : "desc"));
		setTableInfo((prev) => orderBy(prev, [by], [sortOrder()]));
	};

	const formatBytes = (bytes: number, decimals = 2) => {
		return convert(bytes, "bytes").to("best").toString(decimals);
	};

	return (
		<table class="o-table">
			<thead>
				<tr>
					<th>
						<button type="button" class="sort" onClick={[sort, "name"]}>
							Table {sortBy() !== "name" ? "⯁" : sortOrder() === "asc" ? "⯆" : "⯅"}
						</button>
					</th>
					<th colSpan={2}>
						<button type="button" class="sort" onClick={[sort, "size"]}>
							Size {sortBy() !== "size" ? "⯁" : sortOrder() === "asc" ? "⯆" : "⯅"}
						</button>
					</th>
				</tr>
			</thead>
			<tbody>
				<For
					each={tableInfo()}
					fallback={
						<tr>
							<td colSpan={3}>Loading...</td>
						</tr>
					}
				>
					{(item) => (
						<tr>
							<td>{item.name}</td>
							<td>{item.size} B</td>
							<td>{formatBytes(item.size)}</td>
						</tr>
					)}
				</For>
			</tbody>
		</table>
	);
};

customElement("table-info", {}, Styled(TableInfo, css));
