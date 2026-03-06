import { describe, expect, test } from "bun:test";
import { bytesToHex, hexToBytes } from "@h/byte-hex-helpers";

describe("hexToBytes", () => {
	test("converts a simple hex string", () => {
		expect(hexToBytes("deadbeef")).toEqual(new Uint8Array([0xde, 0xad, 0xbe, 0xef]));
	});

	test("handles 0x prefix", () => {
		expect(hexToBytes("0xdeadbeef")).toEqual(new Uint8Array([0xde, 0xad, 0xbe, 0xef]));
	});

	test("handles 0X prefix", () => {
		expect(hexToBytes("0Xdeadbeef")).toEqual(new Uint8Array([0xde, 0xad, 0xbe, 0xef]));
	});

	test("handles odd-length hex string (missing leading zero)", () => {
		// "f" should be treated as "0f"
		expect(hexToBytes("f")).toEqual(new Uint8Array([0x0f]));
	});

	test("handles odd-length hex string with 0x prefix", () => {
		expect(hexToBytes("0xfff")).toEqual(new Uint8Array([0x0f, 0xff]));
	});

	test("produces 32 bytes for a 64-char hex string", () => {
		const hex = "000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
		expect(hexToBytes(hex).length).toBe(32);
	});

	test("produces 32 bytes for a 63-char hex string (odd length)", () => {
		// BigInt.toString(16) can produce 63 chars for some 256-bit values
		const hex = "00ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
		expect(hexToBytes(hex).length).toBe(32);
	});

	test("handles all-zero input", () => {
		expect(hexToBytes("0000")).toEqual(new Uint8Array([0x00, 0x00]));
	});

	test("handles all-ff input", () => {
		expect(hexToBytes("ffff")).toEqual(new Uint8Array([0xff, 0xff]));
	});

	test("handles empty string", () => {
		expect(hexToBytes("")).toEqual(new Uint8Array([]));
	});

	test("is case-insensitive", () => {
		expect(hexToBytes("DEADBEEF")).toEqual(hexToBytes("deadbeef"));
	});
});

describe("bytesToHex", () => {
	test("converts simple bytes", () => {
		expect(bytesToHex(new Uint8Array([0xde, 0xad, 0xbe, 0xef]))).toBe("deadbeef");
	});

	test("pads single-digit hex values with leading zero", () => {
		expect(bytesToHex(new Uint8Array([0x00, 0x0f, 0x10]))).toBe("000f10");
	});

	test("handles all-zero bytes", () => {
		expect(bytesToHex(new Uint8Array([0x00, 0x00]))).toBe("0000");
	});

	test("handles all-ff bytes", () => {
		expect(bytesToHex(new Uint8Array([0xff, 0xff]))).toBe("ffff");
	});

	test("handles empty array", () => {
		expect(bytesToHex(new Uint8Array([]))).toBe("");
	});

	test("output is always lowercase", () => {
		const hex = bytesToHex(new Uint8Array([0xab, 0xcd, 0xef]));
		expect(hex).toBe(hex.toLowerCase());
	});

	test("output length is always 2x input length", () => {
		const bytes = new Uint8Array([0x01, 0x02, 0x03, 0x04]);
		expect(bytesToHex(bytes).length).toBe(bytes.length * 2);
	});
});

describe("round-trip", () => {
	test("hexToBytes -> bytesToHex returns original hex", () => {
		const hex = "deadbeefcafebabe";
		expect(bytesToHex(hexToBytes(hex))).toBe(hex);
	});

	test("bytesToHex -> hexToBytes returns original bytes", () => {
		const bytes = new Uint8Array([0x00, 0x11, 0x22, 0xff]);
		expect(hexToBytes(bytesToHex(bytes))).toEqual(bytes);
	});

	test("round-trip preserves 32-byte PoW target", () => {
		const hex = "000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
		const result = bytesToHex(hexToBytes(hex));
		expect(result).toBe(hex);
		expect(hexToBytes(result).length).toBe(32);
	});
});
