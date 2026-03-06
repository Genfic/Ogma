import { bytesToHex } from "@h/byte-hex-helpers";
import * as Comlink from "comlink";
import { createSHA256 } from "hash-wasm";

let hasher: Awaited<ReturnType<typeof createSHA256>> | null = null;

const checkHash = (bytes: Uint8Array, target: Uint8Array) => {
	if (bytes.length !== target.length) {
		return false;
	}
	for (let i = 0; i < target.length; i++) {
		if (bytes[i] < target[i]) {
			return true;
		}
		if (bytes[i] > target[i]) {
			return false;
		}
	}
	return true;
};

const api = {
	/**
	 * Mines a hash that matches the target criteria by iterating over nonce values and checking the hash.
	 *
	 * @param {string} data The input data to include in the hash calculation along with the nonce.
	 * @param {Uint8Array} target The target threshold used to validate the generated hash.
	 * @return {Promise<{nonce: number, hash: string}>} A promise that resolves to an object containing the valid nonce and corresponding hash.
	 */
	async mine(data: string, target: Uint8Array) {
		hasher ??= await createSHA256();

		let nonce = 0;
		let lastYield = performance.now();

		while (true) {
			hasher.init();
			hasher.update(`${data}${nonce}`);

			const hash = hasher.digest("binary");

			if (checkHash(hash, target)) {
				return { nonce, hash: bytesToHex(hash) };
			}

			nonce++;

			if (performance.now() - lastYield > 16) {
				await new Promise((resolve) => setTimeout(resolve, 0));
				lastYield = performance.now();
			}
		}
	},
};

export type PowApi = typeof api;
Comlink.expose(api);
