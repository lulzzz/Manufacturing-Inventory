using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Domain.Security.Concrete {

    public class EncryptedResult {
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
        public byte[] Encrypted { get; set; }
    }

    public static class EncryptionService {
        public static async Task<EncryptedResult> Encrypt(string password) {
            EncryptedResult result = new EncryptedResult();
            using (Aes aes = Aes.Create()) {
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryEncrypt = new MemoryStream()) {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryEncrypt, encryptor, CryptoStreamMode.Write)) {
                        using (StreamWriter writerEncrypt = new StreamWriter(cryptoStream)) {
                            await writerEncrypt.WriteAsync(password);
                        }
                        result.Encrypted = memoryEncrypt.ToArray();
                        result.IV = aes.IV;
                        result.Key = aes.Key;
                    }
                }
            }
            return result;
        }

        public static async Task<string> Decrypt(EncryptedResult encryptedResult) {
            string decrypted;
            using (Aes aes = Aes.Create()) {
                aes.Key = encryptedResult.Key;
                aes.IV = encryptedResult.IV;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryDecrypt = new MemoryStream(encryptedResult.Encrypted)) {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryDecrypt, decryptor, CryptoStreamMode.Read)) {
                        using (StreamReader readerDecrypt = new StreamReader(cryptoStream)) {
                            decrypted = await readerDecrypt.ReadToEndAsync();
                        }
                    }
                }
            }
            return decrypted;
        }
    }
}
