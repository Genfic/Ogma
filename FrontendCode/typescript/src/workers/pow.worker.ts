import { bytesToHex } from "@h/byte-hex-helpers";
import * as Comlink from "comlink";
import { createSHA256 } from "hash-wasm";

const encoder = new TextEncoder();

const checkHash = (hash: Uint8Array, target: Uint8Array): boolean => {
	for (let i = 0; i < target.length; i++) {
		if (hash[i] < target[i]) {
			return true;
		}
		if (hash[i] > target[i]) {
			return false;
		}
	}

	return true;
};

let shouldAbort = false;

const api = {
	async mine(
		data: string,
		target: Uint8Array,
		onLog: (text: string) => void,
	): Promise<{ nonce: number; hash: string }> {
		shouldAbort = false;
		const start = performance.now();

		const hasher = await createSHA256();
		const prefix = encoder.encode(data);

		let nonce = 0;
		let lastYield = performance.now();

		onLog("Mining started.");

		while (!shouldAbort) {
			hasher.init();
			hasher.update(prefix);
			hasher.update(encoder.encode(nonce.toString()));

			const hash = hasher.digest("binary");

			if (checkHash(hash, target)) {
				onLog(`Mining completed in ${performance.now() - start} ms`);
				shouldAbort = true;
				return {
					nonce,
					hash: bytesToHex(hash),
				};
			}

			nonce++;

			if (performance.now() - lastYield >= 100) {
				onLog(`Mining nonce ${nonce}`);
				await new Promise<void>((resolve) => setTimeout(resolve, 0));
				lastYield = performance.now();
			}
		}

		onLog("Mining aborted");
		throw new Error("Mining aborted");
	},

	abort() {
		shouldAbort = true;
	},
};

export type PowApi = typeof api;

Comlink.expose(api);
