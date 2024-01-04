export async function computeHash(data: string) {
    const encoder = new TextEncoder();
    const utf8 = encoder.encode(data);
    const hashBuffer = await crypto.subtle.digest("SHA-384", utf8);
    const hashArray = Array.from(new Uint8Array(hashBuffer));
    const hashHex = hashArray.map(byte => byte.toString(16).padStart(2, '0')).join('');
    return hashHex;
  }
  