export const hexToBytes = (hex: string): Uint8Array => {
	let h = hex;

	if (h.startsWith("0x") || h.startsWith("0X")) {
		h = h.slice(2);
	}
	if (h.length % 2 !== 0) {
		h = h.padStart(h.length + 1, "0");
	}
	const bytes = new Uint8Array(h.length / 2);
	for (let i = 0; i < bytes.length; i++) {
		bytes[i] = Number.parseInt(h.slice(i * 2, i * 2 + 2), 16);
	}
	return bytes;
};

export const bytesToHex = (bytes: Uint8Array): string =>
	Array.from(bytes, (b) => b.toString(16).padStart(2, "0")).join("");
