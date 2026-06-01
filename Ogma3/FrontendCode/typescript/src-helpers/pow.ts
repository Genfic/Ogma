import { hexToBytes } from "@h/byte-hex-helpers";
import * as Comlink from "comlink";
import type { PowApi } from "../src/workers/pow.worker";

let cache: { worker?: Worker; api?: Comlink.Remote<PowApi> } = {};

const getWorkerApi = () => {
	if (cache.worker && cache.api) {
		return cache.api;
	}
	const worker = new Worker(new URL("/js/workers/pow.worker.js", import.meta.url), { type: "module" });
	const api = Comlink.wrap<PowApi>(worker);
	cache = {
		worker,
		api,
	};
	return api;
};

export type PowResult = {
	nonce: number;
	hash: string;
};

export const minePow = async (payload: string, difficulty: number): Promise<PowResult> => {
	const api = getWorkerApi();

	try {
		await api.abort();
	} catch (e) {
		console.error("Abort failed", e);
	}

	const max = 0xffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffn;
	const target = max >> BigInt(difficulty);

	const targetHex = target.toString(16).padStart(64, "0");

	return api.mine(payload, hexToBytes(targetHex));
};
