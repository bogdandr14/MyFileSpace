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

        public static byte[] HashKeyWithSalt(string passphrase, byte[] salt, int keySize = KEY_SIZE)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(passphrase),
                salt,
                ITERATIONS,
                HASH_ALGORITHM,
                keySize);
        }

        public static string HashKeyWithSaltString(string passphrase, byte[] salt)
        {
            byte[] hash = HashKeyWithSalt(passphrase, salt);
            return Convert.ToHexString(hash);
        }

        public static bool VerifyKey(string passphrase, string hash, string saltString)
        {
            byte[] salt = Convert.FromHexString(saltString);
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(passphrase, salt, ITERATIONS, HASH_ALGORITHM, KEY_SIZE);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }

        public static async Task<string> EncryptAsync(string clearText, string passphrase)
        {
            byte[] encryptedBytes = (await EncryptAsync(Encoding.Unicode.GetBytes(clearText), passphrase));
            return Encoding.Unicode.GetString(encryptedBytes);
        }

        public static async Task<string> DecryptAsync(string encryptedText, string passphrase)
        {
            byte[] decryptedBytes = (await DecryptAsync(Encoding.Unicode.GetBytes(encryptedText), passphrase));
            return Encoding.Unicode.GetString(decryptedBytes);
        }

        public static async Task<byte[]> EncryptAsync(byte[] unencryptedBytes, string passphrase)
        {
            using MemoryStream output = new();
            await EncryptAsync(output, unencryptedBytes, passphrase);
            return output.ToArray();
        }

        public static async Task<byte[]> EncryptAsyncStream(byte[] unencryptedBytes, string passphrase)
        {
            using MemoryStream output = new();

            await EncryptAsync(output, unencryptedBytes, passphrase);
            return output.ToArray();
        }

        public static async Task EncryptAsync(Stream outputStream, byte[] unencryptedBytes, string passphrase)
        {
            using CryptoStream cryptoStream = GetWriteCrytoStream(outputStream, passphrase);
            await cryptoStream.WriteAsync(unencryptedBytes);
            await cryptoStream.FlushFinalBlockAsync();
        }

        public static async Task<byte[]> DecryptAsync(byte[] encryptedBytes, string passphrase)
        {
            MemoryStream inputStream = new MemoryStream(encryptedBytes);
            using CryptoStream cryptoStream = GetReadCrytoStream(inputStream, passphrase);
            using MemoryStream output = new();
            await cryptoStream.CopyToAsync(output);
            return output.ToArray();
        }

        public static CryptoStream GetWriteCrytoStream(Stream stream, string passphrase)
        {
            return CreateCryptoStream(stream, CryptoStreamMode.Write, passphrase);
        }

        public static CryptoStream GetReadCrytoStream(Stream stream, string passphrase)
        {
            return CreateCryptoStream(stream, CryptoStreamMode.Read, passphrase);
        }

        private static CryptoStream CreateCryptoStream(Stream stream, CryptoStreamMode cryptoStreamMode, string passphrase)
        {
            using Aes aes = Aes.Create();
            aes.Key = HashKeyWithSalt(passphrase, Array.Empty<byte>(), 32);
            aes.IV = AES_IV;
            return new CryptoStream(stream, aes.CreateEncryptor(), cryptoStreamMode);
        }
    }
}
