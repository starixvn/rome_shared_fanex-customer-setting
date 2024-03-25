declare const CryptoJS: any;

export function encrypt(plainText: string, secretKey: string): string {
  const passPhrase = 'a5s8d2e9c172';
  const key = CryptoJS.enc.Utf8.parse(passPhrase + secretKey);
  const iv = key;

  const pwd = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(plainText), key, {
    keySize: 128 / 8,
    iv: iv,
    mode: CryptoJS.mode.CBC,
    padding: CryptoJS.pad.Pkcs7,
  });

  return pwd.toString();
}
