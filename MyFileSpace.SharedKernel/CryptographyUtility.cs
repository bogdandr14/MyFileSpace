using System.Security.Cryptography;
using System.Text;

namespace MyFileSpace.SharedKernel
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
            if (clearText.Length / 2 == 1)
            {
                clearText = clearText + '0';
            }
            using MemoryStream unencryptedInput = new(Convert.FromHexString(clearText));
            return Convert.ToHexString(await EncryptAsync(unencryptedInput, passphrase));
        }

        public static async Task<byte[]> EncryptAsync(Stream inputDecryptedStream, string passphrase)
        {
            using MemoryStream encryptedOutput = new();
            await EncryptAsync(inputDecryptedStream, passphrase, encryptedOutput);
            return encryptedOutput.ToArray();
        }

        public static async Task EncryptAsync(Stream inputDecryptedStream, string passphrase, Stream outputStream)
        {
            using Aes aes = CreateAes(passphrase);
            using CryptoStream cryptoStream = new CryptoStream(inputDecryptedStream, aes.CreateEncryptor(), CryptoStreamMode.Read);
            await cryptoStream.CopyToAsync(outputStream);
        }

        public static async Task<string> DecryptAsync(string encryptedText, string passphrase)
        {
            MemoryStream encryptedInput = new MemoryStream(Convert.FromHexString(encryptedText));
            return Convert.ToHexString((await DecryptAsync(encryptedInput, passphrase)).ToArray());
        }

        public static async Task<MemoryStream> DecryptAsync(Stream inputEncryptedStream, string passphrase)
        {
            MemoryStream decryptedOutput = new();
            await DecryptAsync(inputEncryptedStream, passphrase, decryptedOutput);
            return decryptedOutput;
        }

        public static async Task DecryptAsync(Stream inputEncryptedStream, string passphrase, Stream outputStream)
        {
            using Aes aes = CreateAes(passphrase);
            using CryptoStream cryptoStream = new CryptoStream(inputEncryptedStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            await cryptoStream.CopyToAsync(outputStream);
        }

        #region "Helper methods"
        private static Aes CreateAes(string passphrase)
        {
            Aes aes = Aes.Create();
            aes.Key = HashKeyWithSalt(passphrase, Array.Empty<byte>(), 32);
            aes.IV = AES_IV;
            return aes;
        }
        #endregion
    }
}
