using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MyFileSpace.SharedKernel.Helpers
{
    public static class CryptographyUtility
    {
        private const int KEY_SIZE = 64;
        private const int ITERATIONS = 214379;
        private const int SALT_SIZE = 32;
        private static HashAlgorithmName HASH_ALGORITHM = HashAlgorithmName.SHA512;
        private static byte[] AES_IV = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
        
        public static string HashKey(string passphrase, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(SALT_SIZE);
            return HashKeyWithSaltString(passphrase, salt);
        }
        public static string HashKey(string passphrase, out string saltString)
        {
            string hashString = HashKey(passphrase, out byte[] salt);
            saltString = Convert.ToHexString(salt);
            return hashString;
        }

        public static byte[] HashKeyWithSalt(string passphrase, byte[] salt)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(passphrase),
                salt,
                ITERATIONS,
                HASH_ALGORITHM,
                KEY_SIZE);
        }

        public static string HashKeyWithSaltString(string passphrase, byte[] salt)
        {
            byte[] hash = HashKeyWithSalt(passphrase, salt);
            return Convert.ToHexString(hash);
        }

        public static bool VerifyKey(string passphrase, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(passphrase, salt, ITERATIONS, HASH_ALGORITHM, KEY_SIZE);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }

        public static async Task<byte[]> EncryptAsync(string clearText, string passphrase)
        {
            using Aes aes = Aes.Create();
            aes.Key = HashKeyWithSalt(passphrase, Array.Empty<byte>());
            aes.IV = AES_IV;
            using MemoryStream output = new();
            using CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write);
            await cryptoStream.WriteAsync(Encoding.Unicode.GetBytes(clearText));
            await cryptoStream.FlushFinalBlockAsync();
            return output.ToArray();
        }

        public static async Task<string> DecryptAsync(byte[] encrypted, string passphrase)
        {
            using Aes aes = Aes.Create();
            aes.Key = HashKeyWithSalt(passphrase, Array.Empty<byte>());
            aes.IV = AES_IV;
            using MemoryStream input = new(encrypted);
            using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using MemoryStream output = new();
            await cryptoStream.CopyToAsync(output);
            return Encoding.Unicode.GetString(output.ToArray());
        }
    }
}
