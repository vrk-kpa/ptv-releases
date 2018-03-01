/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
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
