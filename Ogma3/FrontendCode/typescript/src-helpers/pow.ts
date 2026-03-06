import { hexToBytes } from "@h/byte-hex-helpers";
import * as Comlink from "comlink";
import type { PowApi } from "../src/workers/pow.worker";

export const pow = async (payload: string, difficulty: number) => {
	const worker = new Worker(new URL("/js/workers/pow.worker.js", import.meta.url), { type: "module" });
	const api = Comlink.wrap<PowApi>(worker);

	const max = 0xffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffn;
	const target = max >> BigInt(difficulty);

	const targetHex = target.toString(16).padStart(64, "0");

	try {
		return await api.mine(payload, hexToBytes(targetHex));
	} finally {
		api[Comlink.releaseProxy]();
		worker.terminate();
	}
};
