using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PleasantlyGames.RPG.Runtime.Save.DataTransformers
{
    public class AesDataEncryption : DataTransformerDecorator
    {
        private readonly byte[] _key;
        
        public AesDataEncryption(IDataTransformer wrappedTransformer, string password) : base(wrappedTransformer)
        {
            using var sha = SHA256.Create();
            _key = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public override string Transform(string data)
        {
            data = base.Transform(data);
            return Encrypt(data);
        }

        public override string Reverse(string data)
        {
            data = Decrypt(data);
            return base.Reverse(data);
        }

        private string Encrypt(string input)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            using var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            using var outputStream = new MemoryStream();
            
            outputStream.Write(aes.IV, 0, aes.IV.Length);

            using (var cryptoStream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write))
                inputStream.CopyTo(cryptoStream);

            return Convert.ToBase64String(outputStream.ToArray());
        }
        
        private string Decrypt(string input)
        {
            if (string.IsNullOrEmpty(input)) return default;
            var fullCipher = Convert.FromBase64String(input);

            using var aes = Aes.Create();
            aes.Key = _key;
            
            var iv = new byte[16];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var inputStream = new MemoryStream(fullCipher, 16, fullCipher.Length - 16);
            using var cryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read);
            using var outputStream = new MemoryStream();

            cryptoStream.CopyTo(outputStream);
            return Encoding.UTF8.GetString(outputStream.ToArray());
        }
    }
}