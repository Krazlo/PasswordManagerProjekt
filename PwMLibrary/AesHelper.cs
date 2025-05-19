using System.Security.Cryptography;
using System.Text.Json;
using System.Text;
using static PwM_Library.Argon2Helper;

namespace PwMLibrary
{
    public static class AesHelper
    {
        private static readonly Encoding encoding = Encoding.UTF8;


        /// <summary>
        /// AES Encryption
        /// </summary>
        /// <param name="plainText">String input for encryption.</param>
        /// <param name="password">Master Password</param>>
        /// <returns>string</returns>
        public static string Encrypt(string plainText, string password)
        {
            try
            {
                var aes = Aes.Create();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                aes.Key = Argon2.Argon2HashPassword(password);
                aes.GenerateIV();

                var AESEncrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                var buffer = encoding.GetBytes(plainText);
                var encryptedText = Convert.ToBase64String(AESEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
                var mac = "";
                mac = BitConverter.ToString(HmacSHA256(Convert.ToBase64String(aes.IV) + encryptedText, password)).Replace("-", "").ToLower();
                var keyValues = new Dictionary<string, object>
                {
                    { "iv", Convert.ToBase64String(aes.IV) },
                    { "value", encryptedText },
                    { "mac", mac },
                };
                Argon2.s_argon2.Reset();
                Argon2.s_argon2.Dispose();
                return Convert.ToBase64String(encoding.GetBytes(JsonSerializer.Serialize(keyValues)));
            }
            catch (Exception e)
            {
                return "Error encrypting: " + e.Message;
            }
        }

        /// <summary>
        /// AES Decryption 
        /// </summary>
        /// <param name="plainText">String input for decryption</param>
        /// <param name="password">Master Password</param>
        /// <returns>string</returns>
        public static string Decrypt(string plainText, string password)
        {
            try
            {
                var aes = Aes.Create();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                aes.Key = Argon2.Argon2HashPassword(password);
               
                var base64Decoded = Convert.FromBase64String(plainText);
                var base64DecodedStr = encoding.GetString(base64Decoded);
                var payload = JsonSerializer.Deserialize<Dictionary<string, string>>(base64DecodedStr);
                string expectedMac = BitConverter.ToString(HmacSHA256(payload["iv"] + payload["value"], password)).Replace("-", "").ToLower();
                if (expectedMac != payload["mac"])
                {
                    throw new ApplicationException("Error (AesHelper.Decrypt) - MAC mismatch");
                }

                aes.IV = Convert.FromBase64String(payload["iv"]);
                var AESDecrypt = aes.CreateDecryptor(aes.Key, aes.IV);
                var buffer = Convert.FromBase64String(payload["value"]);
                Argon2.s_argon2.Reset();
                Argon2.s_argon2.Dispose();
                return encoding.GetString(AESDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (Exception e)
            {
                return "Error decrypting: " + e.Message;
            }
        }

        /// <summary>
        /// Hash computation with SHA256
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static byte[] HmacSHA256(String data, String key)
        {
            using (var hmac = new HMACSHA256(encoding.GetBytes(key)))
            {
                return hmac.ComputeHash(encoding.GetBytes(data));
            }
        }
    }
}

