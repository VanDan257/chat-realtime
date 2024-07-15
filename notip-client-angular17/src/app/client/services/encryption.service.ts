import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import * as JSEncrypt from 'jsencrypt';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EncryptionService {
  private key: string = 'super secret key protect your message';

  encrypt(message: string): string {
    return CryptoJS.AES.encrypt(message, this.key).toString();
  }

  decrypt(encryptedMessage: string): string {
    const bytes = CryptoJS.AES.decrypt(encryptedMessage, this.key);
    return bytes.toString(CryptoJS.enc.Utf8);
  }

//   private publicKey: string = ` -----BEGIN PUBLIC KEY-----
// MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAtM85DR4WvH47pdRy6vYN
// CnToUQ56GGxZgbNxaIPXoSfznuVdbl+UwLS9qeLETj+6b2w+60uqCgIDenhyJ3/Q
// SPHqd7+HARtAkLPMnNPyoEtCk1XYOtqnBw7LNhGUJBL3leYVsnp9IyNED8vFdimH
// G9af3mXjswUxx0pDSaHm5hEKlxB/Bz8N3mtndl74tFzjaRuJebDO6y55LnqxrLF/
// 3JsxF5tRxTGvBGaeXy+7mDe80KIxajNLrW2OPf7ScEUwFXOTCdGttmAiqkbllp1q
// z1VHiMF237+e2fGYvZEpsdUShoqViJXw1QIN3sQUlux7Y43OkGpUXsHgdEQcKPhv
// wwIDAQAB
// -----END PUBLIC KEY-----`;

//   encryptMessage(message: string) {
//     if (!this.publicKey) {
//       throw new Error('Public key not loaded');
//     }

//     // Tạo khóa AES ngẫu nhiên có độ dài chính xác là 32 byte
//     const aesKey = CryptoJS.lib.WordArray.random(32 / 2).toString(CryptoJS.enc.Hex);

//     const encryptedMessage = CryptoJS.AES.encrypt(message, CryptoJS.enc.Hex.parse(aesKey), {
//       mode: CryptoJS.mode.ECB,
//       padding: CryptoJS.pad.Pkcs7
//     }).toString();

//     const encrypt = new JSEncrypt.JSEncrypt();
//     encrypt.setPublicKey(this.publicKey);
//     const encryptedKey = encrypt.encrypt(aesKey);

//     if (!encryptedKey) {
//       throw new Error('Encryption failed');
//     }

//     return { encryptedMessage, encryptedKey };
//   }
}
