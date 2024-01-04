import { BigInteger } from "jsbn";

function BigIntFromBuffer(buffer: Uint8Array) {
	const hashArray = Array.from(buffer);
	const hashHex = hashArray
		.map((bytes) => bytes.toString(16).padStart(2, "0"))
		.join("");
	return new BigInteger(hashHex, 16);
}

function secureRandom() {
	const randomBuffer = new Uint8Array(256);
	window.crypto.getRandomValues(randomBuffer);
	return randomBuffer;
}

async function hashMessage(message: string) {
	const utf8 = new TextEncoder().encode(message);
	const hashBuffer = await crypto.subtle.digest("SHA-384", utf8);
	return BigIntFromBuffer(new Uint8Array(hashBuffer));
}

function findBlindingFactor(N: BigInteger) {
	const bigOne = new BigInteger("1");
	let gcd;
	let r;
	do {
		const rand = secureRandom();
		r = BigIntFromBuffer(rand).mod(N);
		gcd = r.gcd(N);
	} while (
		!gcd.equals(bigOne) ||
		r.compareTo(N) >= 0 ||
		r.compareTo(bigOne) <= 0
	);

	return r;
}

async function blind(message: string, n: string, e: string) {
	const messageHash = await hashMessage(message);
	const E = new BigInteger(e);
	const N = new BigInteger(n);
	const r = findBlindingFactor(N);

	const blinded = messageHash.multiply(r.modPow(E, N)).mod(N); // m' = m * r^e (mod N)
	const blindedMessage = blinded.toString();
	return {
		blindedMessage,
		r,
	};
}

function unblind(signed: string, r: BigInteger, n: string) {
	const N = new BigInteger(n);
	const signedData = new BigInteger(signed);
	const unblinded = signedData.multiply(r.modInverse(N)).mod(N); // s = s' * r^-1  (mod N)
	return unblinded.toString();
}

export { blind, unblind };
