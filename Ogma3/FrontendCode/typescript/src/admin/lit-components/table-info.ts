import { GetAdminApiTelemetryGetTableInfo } from "@g/paths-internal";
import { convert } from "convert";
import { LitElement, css, html } from "lit";
import { customElement, state } from "lit/decorators.js";
import { map } from "lit/directives/map.js";
import { when } from "lit/directives/when.js";

@customElement("table-info")
export class TableInfo extends LitElement {
	@state() private sortBy: "size" | "name" = "size";
	@state() private sortOrder: "asc" | "desc" = "asc";
	@state() private tableInfo: { name: string; size: number }[] = [];

	// language=CSS
	static styles = css`
			table {
				border-collapse: collapse;
				outline: 1px solid var(--foreground-50);
			}
			tr:hover {
				background-color: var(--accent-10);
			}
			td {
				border: 1px solid var(--foreground-25);
				padding: 0.5rem;
			}
			th {
				cursor: pointer;
				user-select: none;
			}
		`;

	async connectedCallback() {
		super.connectedCallback();
		this.classList.add("wc-loaded");
		await this._load();
	}

	render() {
		return html`
			<table class="o-table">
				<tr>
					<th @click="${() => this._sort("name")}">
						Table ${this.sortBy !== "name" ? "⯁" : this.sortOrder === "asc" ? "⯆" : "⯅"}
					</th>
					<th colspan="2" @click="${() => this._sort("size")}">
						Size ${this.sortBy !== "size" ? "⯁" : this.sortOrder === "asc" ? "⯆" : "⯅"}
					</th>
				</tr>
				${when(
					this.tableInfo,
					() =>
						map(
							this.tableInfo,
							(i) => html`
								<tr>
									<td>${i.name}</td>
									<td>${i.size} B</td>
									<td>${this._formatBytes(i.size)}</td>
								</tr>
							`,
						),
					() =>
						html`<tr>
							<td colspan="3">Loading...</td>
						</tr>`,
				)}
			</table>
		`;
	}

	private _load = async () => {
		const res = await GetAdminApiTelemetryGetTableInfo();
		if (!res.ok) return;
		const data = res.data;
		this.tableInfo = Object.entries(data).map(([k, v]) => ({
			name: k,
			size: Number.parseInt(v),
		}));
	};

	private _sort(by: "size" | "name") {
		this.sortBy = by;
		this.sortOrder = this.sortOrder === "desc" ? "asc" : "desc";
		if (by === "size") {
			this.tableInfo.sort((a, b) => (this.sortOrder === "desc" ? a.size - b.size : b.size - a.size));
		} else {
			this.tableInfo.sort((a, b) =>
				this.sortOrder === "desc" ? a.name.localeCompare(b.name) : b.name.localeCompare(a.name),
			);
		}
	}

	private _formatBytes(bytes: number, decimals = 2) {
		const num = convert(bytes, "bytes").to("best");
		return `${num.quantity.toFixed(Math.abs(decimals))} ${num.unit}`;
	}
}
