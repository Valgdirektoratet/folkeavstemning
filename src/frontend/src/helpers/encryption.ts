function toBase64(buf: ArrayBuffer) {
	const array = [...new Uint8Array(buf)];
	const data = String.fromCharCode.apply(null, array);
	return window.btoa(data);
}

function fromBase64(base64: string) {
	const str = window.atob(base64);
	const buf = new ArrayBuffer(str.length);
	const bufView = new Uint8Array(buf);
	for (let i = 0; i < str.length; i++) {
		bufView[i] = str.charCodeAt(i);
	}
	return buf;
}

function getKeyData(pem: string) {
	// Strip "-----BEGIN PUBLIC KEY-----" and "-----END PUBLIC KEY-----"
    const start = pem.indexOf("\n");
    const end = pem.lastIndexOf("-----END PUBLIC KEY-----")
    const content = pem.substring(start, end);
    return fromBase64(content);
}

async function importPublicKey(pem: string) {
	const keyData = getKeyData(pem);
	const algorithm = { name: "RSA-OAEP", hash: "SHA-384" };
	return await window.crypto.subtle.importKey(
		"spki",
		keyData,
		algorithm,
		true,
		["encrypt"],
	);
}

async function encryptRSA(key: CryptoKey, data: BufferSource) {
	const algorithm = { name: "RSA-OAEP" };
	return await window.crypto.subtle.encrypt(algorithm, key, data);
}

async function encrypt(data: string, publicKeyPem: string) {
	const pub = await importPublicKey(publicKeyPem);
	const encrypted = await encryptRSA(pub, new TextEncoder().encode(data));
	return toBase64(encrypted);
}

export { encrypt };
