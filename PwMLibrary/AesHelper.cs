using System.Security.Cryptography;
using System.Text.Json;
using System.Text;
using static PwM_Library.Argon2Helper;
using PwM_Library;

namespace PwMLibrary
{
    public static class AesHelper
    {
        private static readonly Encoding encoding = Encoding.UTF8;


        public static string Encrypt(string plainText, string password)
        {
            try
            {
                // Generate secure random salt
                byte[] salt = new byte[16];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(salt);

                // Derive keys
                byte[] aesKey = Argon2Helper.DeriveKey(password, salt, "encryption");
                byte[] hmacKey = Argon2Helper.DeriveKey(password, salt, "authentication");

                // Setup AES
                using var aes = Aes.Create();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                aes.Key = aesKey;
                aes.GenerateIV();

                // Encrypt
                var encryptor = aes.CreateEncryptor();
                byte[] buffer = encoding.GetBytes(plainText);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
                string encryptedText = Convert.ToBase64String(encryptedBytes);

                // Compute HMAC
                string ivBase64 = Convert.ToBase64String(aes.IV);
                string mac = Convert.ToHexString(HmacSHA256(ivBase64 + encryptedText, hmacKey)).ToLower();

                // Package all values
                var keyValues = new Dictionary<string, string>
                {
                    { "iv", ivBase64 },
                    { "value", encryptedText },
                    { "mac", mac },
                    { "salt", Convert.ToBase64String(salt) }
                };

                return Convert.ToBase64String(encoding.GetBytes(JsonSerializer.Serialize(keyValues)));
            }
            catch (Exception e)
            {
                return "Error encrypting: " + e.Message;
            }
        }


        public static string Decrypt(string encryptedInput, string password)
        {
            try
            {
                // Decode and deserialize
                byte[] decoded = Convert.FromBase64String(encryptedInput);
                string json = encoding.GetString(decoded);
                var payload = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                // Extract fields
                byte[] salt = Convert.FromBase64String(payload["salt"]);
                string ivBase64 = payload["iv"];
                string cipherText = payload["value"];
                string receivedMac = payload["mac"];

                // Derive keys
                byte[] aesKey = Argon2Helper.DeriveKey(password, salt, "encryption");
                byte[] hmacKey = Argon2Helper.DeriveKey(password, salt, "authentication");

                // Verify HMAC
                string expectedMac = Convert.ToHexString(HmacSHA256(ivBase64 + cipherText, hmacKey)).ToLower();
                if (expectedMac != receivedMac)
                {
                    throw new ApplicationException("MAC mismatch: data may have been tampered with.");
                }

                // Decrypt
                using var aes = Aes.Create();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                aes.Key = aesKey;
                aes.IV = Convert.FromBase64String(ivBase64);

                var decryptor = aes.CreateDecryptor();
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                byte[] decrypted = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

                return encoding.GetString(decrypted);
            }
            catch (Exception e)
            {
                return "Error decrypting: " + e.Message;
            }
        }
        private static byte[] HmacSHA256(string data, byte[] key)
        {
            using var hmac = new HMACSHA256(key);
            return hmac.ComputeHash(encoding.GetBytes(data));
        }
    }
}

