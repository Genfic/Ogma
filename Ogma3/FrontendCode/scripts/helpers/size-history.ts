import { appendFile } from "node:fs/promises";

export class SizeHistory {
	private filename = ".sizehist";
	private readonly label;

	constructor(label: string) {
		this.label = label;
	}

	public push = async (size: number) => {
		await appendFile(this.filename, `${this.label}:${Date.now()}:${size}\n`);
	};

	public at = async (index: number): Promise<[size: number, timestamp: Date]> => {
		const file = Bun.file(this.filename);

		if (!(await file.exists())) {
			return [-1, new Date(0)];
		}

		const text = await file.text();
		const lines = text.split("\n").filter((l) => l.startsWith(this.label));
		const line = lines.at(index);
		const [_, stamp, size] = line?.split(":") ?? [null, null, null];
		return [Number.parseInt(size ?? "-1", 10), new Date(stamp ?? 0)];
	};
}
