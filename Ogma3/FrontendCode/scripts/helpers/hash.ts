import type { BunFile, SupportedCryptoAlgorithms } from "bun";

export const getHash = async (file: BunFile, algorithm: SupportedCryptoAlgorithms): Promise<string> => {
	const hasher = new Bun.CryptoHasher(algorithm);

	const stream = file.stream();
	for await (const chunk of stream) {
		hasher.update(chunk);
	}
	const hashBuffer = hasher.digest();
	return Buffer.from(hashBuffer).toString("base64url");
};
