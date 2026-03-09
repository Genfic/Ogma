import { appendFile } from "node:fs/promises";

type Indices<T extends number[]> = { [K in keyof T]: number };

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

	public sizeAt = async <T extends number[]>(...indices: [...T]): Promise<Indices<T>> => {
		const file = Bun.file(this.filename);

		if (!(await file.exists())) {
			return indices.map(() => -1) as Indices<T>;
		}

		const text = await file.text();
		const lines = text.split("\n").filter((l) => l.startsWith(this.label));

		const sizes = lines.map((line) => {
			const size = line.split(":").at(-1);
			return Number.parseInt(size ?? "-1", 10);
		});

		return indices.map((i) => sizes.at(i) ?? -1) as Indices<T>;
	};
}
