export function getNonce() {
	const randomBuffer = new Uint8Array(64);
	window.crypto.getRandomValues(randomBuffer);
	const binaryString = String.fromCharCode.apply(null, [...randomBuffer]);
	return btoa(binaryString);
}
