/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using PTV.Database.DataAccess.Interfaces.Cloud;
using PTV.Domain.Model.Models;
using PTV.Framework;

namespace PTV.Database.DataAccess.Cloud
{
    public class AWSS3Client : IStorageClient
    {
        private readonly IAmazonS3 client;
        private readonly string bucketName;
        private const int DefaultMaxKeys = 1000;
        private const string FolderDelimiter = "/";
        
        public int RequestTimeout { get; set; }

        public AWSS3Client(IAmazonS3 client, string bucketName)
        {
            this.client = client;
            this.bucketName = bucketName;
            RequestTimeout = 900000;
        }

        private async Task EnsureBucketExists()
        {
            var response = await client.ListBucketsAsync();
            if (response.Buckets.Any(x => x.BucketName == bucketName))
            {
                return;
            }
            
            var request = new PutBucketRequest { BucketName = bucketName, UseClientRegion = true };
            await client.PutBucketAsync(request);
        }

        public Task<bool> SaveFileAsync(string path, string fileName, string fileContent)
        {
            var cts = new CancellationTokenSource(RequestTimeout);
            return SaveFileAsync(path, fileName, fileContent, cts.Token);
        }

        public async Task<bool> SaveFileAsync(string path, string fileName, string fileContent, CancellationToken token)
        {
            await EnsureBucketExists();
            var keyName = CombineKeyName(path, fileName);
            
            var request = new PutObjectRequest()
            {
                ContentBody = fileContent,
                BucketName = bucketName,
                Key = keyName
            };

            var response = await client.PutObjectAsync(request, token);
            return (int)response.HttpStatusCode < 400;
        }

        public bool SaveFile(string path, string fileName, string fileContent)
        {
            var cts = new CancellationTokenSource(RequestTimeout);
            return SaveFileAsync(path, fileName, fileContent, cts.Token).GetAwaiter().GetResult();
        }

        public Task<List<VmFileInfo>> ListFilesAsync(string path = null, int? count = null)
        {
            var cts = new CancellationTokenSource(RequestTimeout);
            return ListFilesAsync(path, cts.Token, count);
        }

        public async Task<List<VmFileInfo>> ListFilesAsync(string path, CancellationToken token, int? count = null)
        {
            await EnsureBucketExists();
            if (!path.IsNullOrWhitespace() && !path.EndsWith(FolderDelimiter))
            {
                path += FolderDelimiter;
            }
            
            var request = new ListObjectsRequest
            {
                BucketName = bucketName,
                Prefix = path,
                MaxKeys = count ?? DefaultMaxKeys
            };
            var response = await client.ListObjectsAsync(request, token);
            return response.S3Objects.Select(x => new VmFileInfo
            {
                Name = x.Key,
                LastModified = x.LastModified
            }).ToList();
        }

        public List<VmFileInfo> ListFiles(string path = null, int? count = null)
        {
            var cts = new CancellationTokenSource(RequestTimeout);
            return ListFilesAsync(path, cts.Token, count).GetAwaiter().GetResult();
        }

        public Task<string> ReadFileAsync(string path, string fileName)
        {
            var cts = new CancellationTokenSource(RequestTimeout);
            return ReadFileAsync(path, fileName, cts.Token);
        }

        public async Task<string> ReadFileAsync(string path, string fileName, CancellationToken token)
        {
            await EnsureBucketExists();
            var keyName = CombineKeyName(path, fileName);
            
            var request = new GetObjectRequest()
            {
                BucketName = bucketName,
                Key = keyName
            };

            using (var response = await client.GetObjectAsync(request, token))
            using (var responseStream = response.ResponseStream)
            using (var reader = new StreamReader(responseStream))
            {
                var result = await reader.ReadToEndAsync();
                return result;
            }
        }

        public string ReadFile(string path, string fileName)
        {
            var cts = new CancellationTokenSource(RequestTimeout);
            return ReadFileAsync(path, fileName, cts.Token).GetAwaiter().GetResult();
        }

        public Task<bool> DeleteFileAsync(string path, string fileName)
        {
            var cts = new CancellationTokenSource(RequestTimeout);
            return DeleteFileAsync(path, fileName, cts.Token);
        }

        public async Task<bool> DeleteFileAsync(string path, string fileName, CancellationToken token)
        {
            await EnsureBucketExists();
            var keyName = CombineKeyName(path, fileName);
            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = keyName
            };
            
            var response = await client.DeleteObjectAsync(request, token);
            return (int)response.HttpStatusCode < 400;
        }

        public bool DeleteFile(string path, string fileName)
        {
            var cts = new CancellationTokenSource(RequestTimeout);
            return DeleteFileAsync(path, fileName, cts.Token).GetAwaiter().GetResult();
        }

        public Task<bool> CopyFileAsync(string sourceFolder, string destFolder, string sourceFile, string destFile = null)
        {
            var cts = new CancellationTokenSource(RequestTimeout);
            return CopyFileAsync(sourceFolder, destFolder, sourceFile, destFile, cts.Token);
        }

        public async Task<bool> CopyFileAsync(string sourceFolder, string destFolder, string sourceFile,
            string destFile, CancellationToken token)
        {
            await EnsureBucketExists();
            var oldKeyName = CombineKeyName(sourceFolder, sourceFile);
            var newKeyName = CombineKeyName(destFolder, destFile ?? sourceFile);
            var request = new CopyObjectRequest
            {
                SourceBucket = bucketName,
                SourceKey = oldKeyName,
                DestinationBucket = bucketName,
                DestinationKey = newKeyName
            };
            var copyResponse = await client.CopyObjectAsync(request, token);
            return (int)copyResponse.HttpStatusCode < 400;
        }

        public bool CopyFile(string sourceFolder, string destFolder, string sourceFile, string destFile = null)
        {
            var cts = new CancellationTokenSource(RequestTimeout);
            return CopyFileAsync(sourceFolder, destFolder, sourceFile, destFile, cts.Token).GetAwaiter().GetResult();
        }

        public Task<bool> MoveFileAsync(string oldFolder, string newFolder, string fileName, string newFileName = null)
        {
            var cts = new CancellationTokenSource(RequestTimeout);
            return MoveFileAsync(oldFolder, newFolder, fileName, newFileName, cts.Token);
        }

        public async Task<bool> MoveFileAsync(string oldFolder, string newFolder, string fileName, string newFileName, CancellationToken token)
        {
            await EnsureBucketExists();
            var copyResponse = await CopyFileAsync(oldFolder, newFolder, fileName, newFileName, token);
            if (copyResponse)
            {
                var deleteResponse = await DeleteFileAsync(oldFolder, fileName, token);
                return deleteResponse;
            }

            return false;
        }

        public bool MoveFile(string oldFolder, string newFolder, string fileName, string newFileName = null)
        {
            var cts = new CancellationTokenSource(RequestTimeout);
            return MoveFileAsync(oldFolder, newFolder, fileName, newFileName, cts.Token).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            client?.Dispose();
        }

        private string CombineKeyName(string path, string fileName)
        {
            var keyName = "";
            if (!path.IsNullOrWhitespace())
            {
                keyName = path;

                if (!path.EndsWith(FolderDelimiter))
                {
                    keyName += FolderDelimiter;
                }
            }

            keyName += fileName;
            return keyName;
        }
    }
}
