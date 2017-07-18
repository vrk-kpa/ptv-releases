using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration.Json;

namespace PTV.Framework
{
    public class EncryptedConfig : JsonConfigurationProvider
    {
        private readonly string encryptionKey;
        public EncryptedConfig(JsonConfigurationSource source, string key) : base((JsonConfigurationSource) source)
        {
            this.encryptionKey = key;
        }

        public override void Load(Stream stream)
        {
            try
            {
                byte[] keyAes = new byte[32];
                byte[] iv = new byte[16];

                using (var keyGenHasher = SHA384.Create())
                {
                    keyGenHasher.Initialize();
                    var wholeField = keyGenHasher.ComputeHash(Encoding.ASCII.GetBytes((string) encryptionKey));
                    Buffer.BlockCopy(wholeField, 0, keyAes, 0, 32);
                    Buffer.BlockCopy(wholeField, 16, iv, 0, 16);
                }
                using (var aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.Key = keyAes;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    using (var encryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        var content = new StreamReader(stream).ReadToEnd();
                        var decodedData = Convert.FromBase64String(content);
                        byte[] hashToValidate = new byte[32];
                        Buffer.BlockCopy(decodedData, decodedData.Length - 32, hashToValidate, 0, 32);
                        using (var memTarget = new MemoryStream())
                        {
                            using (var aesStream = new CryptoStream(memTarget, encryptor, CryptoStreamMode.Write))
                            {
                                aesStream.Write(decodedData, 0, decodedData.Length - 32);
                                aesStream.FlushFinalBlock();
                                aesStream.Flush();
                                memTarget.Seek(0, SeekOrigin.Begin);
                                byte[] crcHash;
                                using (var sha = SHA256.Create())
                                {
                                    sha.Initialize();
                                    crcHash = sha.ComputeHash(memTarget);
                                }
                                if (!hashToValidate.SequenceEqual(crcHash))
                                {
                                    throw new Exception("Invalid key for decrypting appsetting file!");
                                }
                                memTarget.Seek(0, SeekOrigin.Begin);
                                base.Load(memTarget);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Invalid settings file or decrypting key!");
            }
        }
    }
}