using System.Security.Cryptography;
using System.Text;

namespace messanger.Services
{
    public class EncryptionService
    {
        private readonly byte[] _key;

        public EncryptionService(IConfiguration configuration)
        {
            var base64Key = configuration["Encryption:Key"];
            if (string.IsNullOrWhiteSpace(base64Key))
            {
                throw new InvalidOperationException("Encryption key is not configured.");
            }

            try
            {
                _key = Convert.FromBase64String(base64Key);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("Encryption key must be valid Base64.", ex);
            }

            if (_key.Length != 16 && _key.Length != 24 && _key.Length != 32)
            {
                throw new InvalidOperationException("Encryption key must be 128, 192, or 256 bits.");
            }
        }

        public string Encrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Text cannot be empty.", nameof(text));
            }

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();

            var plainBytes = Encoding.UTF8.GetBytes(text);
            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            var resultBytes = new byte[aes.IV.Length + cipherBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, resultBytes, 0, aes.IV.Length);
            Buffer.BlockCopy(cipherBytes, 0, resultBytes, aes.IV.Length, cipherBytes.Length);

            return Convert.ToBase64String(resultBytes);
        }

        public string Decrypt(string encryptedText)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
            {
                throw new ArgumentException("Encrypted text cannot be empty.", nameof(encryptedText));
            }

            var fullBytes = Convert.FromBase64String(encryptedText);

            using var aes = Aes.Create();
            aes.Key = _key;

            var ivLength = aes.BlockSize / 8;
            if (fullBytes.Length <= ivLength)
            {
                throw new InvalidOperationException("Encrypted payload is invalid.");
            }

            var iv = new byte[ivLength];
            var cipherBytes = new byte[fullBytes.Length - ivLength];
            Buffer.BlockCopy(fullBytes, 0, iv, 0, ivLength);
            Buffer.BlockCopy(fullBytes, ivLength, cipherBytes, 0, cipherBytes.Length);

            using var decryptor = aes.CreateDecryptor(aes.Key, iv);
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
