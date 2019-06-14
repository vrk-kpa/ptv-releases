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

namespace PTV.AppSettings.Encrypter
{
    class Program
    {
        static void Main(string[] args)
        {
//                        Console.WriteLine("A");
//                        string userKey = args[2];
//            
//                        byte[] keyAes = new byte[32];
//                        byte[] iv = new byte[16];
//            
//                        using (var keyGenHasher = SHA384.Create())
//                        {
//                            keyGenHasher.Initialize();
//                            var wholeField = keyGenHasher.ComputeHash(Encoding.ASCII.GetBytes(userKey));
//                            Buffer.BlockCopy(wholeField, 0, keyAes, 0, 32);
//                            Buffer.BlockCopy(wholeField, 16, iv, 0, 16);
//                        }
//                        string inputFileName = args[1];
//                        //string outputFileName = args[1];
//                        using (var aes = Aes.Create())
//                        {
//                            aes.KeySize = 256;
//                            aes.Key = keyAes;
//                            aes.IV = iv;
//                            aes.Mode = CipherMode.CBC;
//                            aes.Padding = PaddingMode.PKCS7;
//                            using (var encryptor = aes.CreateDecryptor(aes.Key, aes.IV))
//                                using (var inputFile = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
//                                {
//                                    var content = new StreamReader(inputFile).ReadToEnd();
//                                    var decodedData = Convert.FromBase64String(content);
//                                    byte[] hashToValidate = new byte[32];
//                                    Buffer.BlockCopy(decodedData, decodedData.Length - 32, hashToValidate, 0, 32);
//                                    using (var memTarget = new MemoryStream())
//                                    {
//                                        using (var aesStream = new CryptoStream(memTarget, encryptor, CryptoStreamMode.Write))
//                                        {
//                                            aesStream.Write(decodedData, 0, decodedData.Length - 32);
//                                            aesStream.FlushFinalBlock();
//                                            aesStream.Flush();
//                                            memTarget.Seek(0, SeekOrigin.Begin);
//                                            byte[] crcHash;
//                                            using (var sha = SHA256.Create())
//                                            {
//                                                sha.Initialize();
//                                                crcHash = sha.ComputeHash(memTarget);
//                                            }
//                                            if (!hashToValidate.SequenceEqual(crcHash))
//                                            {
//                                                throw new Exception("Invalid key for decrypting appsetting file!");
//                                            }
//                                            memTarget.Seek(0, SeekOrigin.Begin);
//                                            var s = new StreamReader(memTarget).ReadToEnd();
//                                            Console.WriteLine(s);
//                                            Console.ReadLine();
//                                        }
//                                    }
//                                }
//                        }


            if ((args.Length < 3) || (args.Length > 4))
            {
                Console.WriteLine("Usage: dotnet PTV.AppSettings.Encrypter.dll /option [inputfile] [outputfile] [key]");
                Console.WriteLine("options: /e = encrypt, /d = decrypt");
                return;
            }

            var optionSpecified = (args[0] == "/e") || (args[0] == "/d");
            if ((args.Length == 3) || (optionSpecified && args[0] == "/e"))
            {
                string inputFileName = optionSpecified ? args[1] : args[0];
                string outputFileName = optionSpecified ? args[2] : args[1];
                string userKey = optionSpecified ? args[3] : args[2];
                Console.WriteLine($"Encrypting file '{inputFileName}' to '{outputFileName}'...");

                byte[] keyAes = new byte[32];
                byte[] iv = new byte[16];

                using (var keyGenHasher = SHA384.Create())
                {
                    keyGenHasher.Initialize();
                    var wholeField = keyGenHasher.ComputeHash(Encoding.ASCII.GetBytes(userKey));
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
                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                        using (var inputFile = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
                        {
                            byte[] crcHash;
                            using (var sha = SHA256.Create())
                            {
                                sha.Initialize();
                                crcHash = sha.ComputeHash(inputFile);
                            }

                            inputFile.Seek(0, SeekOrigin.Begin);
                            using (var memData = new MemoryStream())
                            {
                                using (var aesStream = new CryptoStream(memData, encryptor, CryptoStreamMode.Write))
                                {
                                    inputFile.CopyTo(aesStream);
                                    aesStream.FlushFinalBlock();
                                    aesStream.Flush();
                                    memData.Seek(0, SeekOrigin.Begin);
                                    var encryptedBytes = new BinaryReader(memData).ReadBytes((int) memData.Length);
                                    var outBytes = new byte[encryptedBytes.Length + crcHash.Length];
                                    Buffer.BlockCopy(encryptedBytes, 0, outBytes, 0, encryptedBytes.Length);
                                    Buffer.BlockCopy(crcHash, 0, outBytes, encryptedBytes.Length, crcHash.Length);

                                    var base64EncodedEncrypted = Convert.ToBase64String(outBytes);
                                    using (var outputFile = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                                    {
                                        outputFile.SetLength(0);
                                        var outStream = new StreamWriter(outputFile);
                                        outStream.Write(base64EncodedEncrypted);
                                        outStream.Flush();
                                        outputFile.Flush();
                                    }
                                }
                            }
                        }
                }
                Console.WriteLine("Done.");
            }

            if ((args.Length == 4) && (optionSpecified && args[0] == "/d"))
            {
                string inputFileName = optionSpecified ? args[1] : args[0];
                string outputFileName = optionSpecified ? args[2] : args[1];
                string userKey = optionSpecified ? args[3] : args[2];
                Console.WriteLine($"Decrypting file '{inputFileName}' to '{outputFileName}'...");
                byte[] keyAes = new byte[32];
                                        byte[] iv = new byte[16];
                            
                                        using (var keyGenHasher = SHA384.Create())
                                        {
                                            keyGenHasher.Initialize();
                                            var wholeField = keyGenHasher.ComputeHash(Encoding.ASCII.GetBytes(userKey));
                                            Buffer.BlockCopy(wholeField, 0, keyAes, 0, 32);
                                            Buffer.BlockCopy(wholeField, 16, iv, 0, 16);
                                        }

                try
                {
                    try
                    {
                        using (var aes = Aes.Create())
                        {
                            aes.KeySize = 256;
                            aes.Key = keyAes;
                            aes.IV = iv;
                            aes.Mode = CipherMode.CBC;
                            aes.Padding = PaddingMode.PKCS7;
                            using (var encryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                                using (var inputFile = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
                                {
                                    var content = new StreamReader(inputFile).ReadToEnd();
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
                                                throw new Exception("Invalid key for decrypting file!");
                                            }

                                            memTarget.Seek(0, SeekOrigin.Begin);
                                            using (var outputFile = new FileStream(outputFileName, FileMode.Create, FileAccess.ReadWrite))
                                            {
                                                outputFile.SetLength(0);
                                                memTarget.WriteTo(outputFile);
                                                outputFile.Flush();
                                            }
                                        }
                                    }
                                }
                        }
                        Console.WriteLine("Done.");
                    }
                    catch (CryptographicException ce)
                    {
                        throw new Exception($"Invalid key for decrypting file or file is corrupted! Internal message: {ce.Message}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
            }
        }
    }
}
