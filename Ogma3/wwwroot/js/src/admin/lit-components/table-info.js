import { css, html, LitElement } from "lit";

export class TableInfo extends LitElement {
	constructor() {
		super();
		this.sortBy = "size";
		this.sortOrder = "asc";
	}

	static get styles() {
		return css`
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
	}

	static get properties() {
		return {
			tableInfo: { type: Array, attribute: false },
			sortOrder: { type: String, attribute: false },
			sortBy: { type: String, attribute: false },
		};
	}

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
					<th
						colspan="2"
						@click="${() => this._sort("size")}"
					>
						Size ${this.sortBy !== "size" ? "⯁" : this.sortOrder === "asc" ? "⯆" : "⯅"}
					</th>
				</tr>
				${this.tableInfo
					? this.tableInfo.map(
						(i) => html`
								<tr>
									<td>${i.name}</td>
									<td>${i.size} B</td>
									<td>${this._formatBytes(i.size)}</td>
								</tr>
							`,
					  )
					: html` <tr>
							<td colspan="3">Loading...</td>
					  </tr>`}
			</table>
		`;
	}

	async _load() {
		const res = await fetch("/admin/api/telemetry/gettableinfo");
		this.tableInfo = await res.json();
	}

	_sort(by) {
		this.sortBy = by;
		this.sortOrder = this.sortOrder === "desc" ? "asc" : "desc";
		if (by === "size") {
			this.tableInfo.sort((a, b) => (this.sortOrder === "desc" ? a.size - b.size : b.size - a.size));
		} else {
			this.tableInfo.sort((a, b) => (this.sortOrder === "desc" ? a.name.localeCompare(b.name) : b.name.localeCompare(a.name)));
		}
	}

	_formatBytes(bytes, decimals = 2) {
		if (bytes === 0) return "0 B";

		const k = 1024;
		const dm = decimals < 0 ? 0 : decimals;
		const sizes = ["B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];

		const i = Math.floor(Math.log(bytes) / Math.log(k));

		return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + " " + sizes[i];
	}
}

window.customElements.define('table-info', TableInfo);