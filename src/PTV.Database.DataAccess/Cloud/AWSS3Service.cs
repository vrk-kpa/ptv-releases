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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using PTV.Database.DataAccess.Interfaces.Cloud;
using PTV.Framework;

namespace PTV.Database.DataAccess.Cloud
{
    [RegisterService(typeof(IStorageService), RegisterType.Transient)]
    // ReSharper disable once InconsistentNaming
    public class AWSS3Service : IStorageService
    {
        private readonly AWSS3Settings options;

        public AWSS3Service(IOptions<AWSS3Settings> options)
        {
            this.options = options.Value;
        }
        
        public IStorageClient GetClient()
        {
            var sharedFile = new SharedCredentialsFile();
            var profile = GetProfile(sharedFile);
            if (profile != null &&
                AWSCredentialsFactory.TryGetAWSCredentials(profile, sharedFile, out var awsCredentials))
            {
                var innerClient = new AmazonS3Client(awsCredentials, profile.Region);
                return new AWSS3Client(innerClient, options.BucketName);
            }

            return null;
        }
        
        private CredentialProfile GetProfile(SharedCredentialsFile sharedFile)
        {
            if (!sharedFile.TryGetProfile(options.ProfileName, out var result))
            {
                var credentials = new CredentialProfileOptions
                {
                    AccessKey = options.AccessKey,
                    SecretKey = options.SecretKey
                };

                var profile = new CredentialProfile(options.ProfileName, credentials);
                profile.Region = RegionEndpoint.GetBySystemName(options.Region);
                sharedFile.RegisterProfile(profile);

                result = profile;
            }

            return result;
        }
    }
}
