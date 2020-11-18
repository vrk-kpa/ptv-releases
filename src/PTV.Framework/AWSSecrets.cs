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

namespace PTV.Framework
{
    public class AWSSecret : Attribute  
    {  
        public readonly string Name;
        public AWSSecret(string name)  
        {  
            Name = name; 
        }  
    }
    /// <summary>
    /// Model for AWS secrets
    /// </summary>
    public class AWSSecrets
    {
        public AWSSecrets()
        {
            GetType().GetProperties().ForEach(property =>
            {
                property.SetValue(this, Environment.GetEnvironmentVariable(((AWSSecret) property.GetCustomAttributes(true).First()).Name));
            });
        }
        /// <summary>
        /// Database name from AWS secrets
        /// </summary>
        [AWSSecret("DB_NAME")]
        public string DbName { get; set; }
        /// <summary>
        /// Database user name from AWS secrets
        /// </summary>
        [AWSSecret("DB_USERNAME")]
        public string DbUserName { get; set; }
        /// <summary>
        /// Database password from AWS secrets
        /// </summary>
        [AWSSecret("DB_PASSWORD")]
        public string DbPassword { get; set; }
        /// <summary>
        /// Map username from AWS secrets
        /// </summary>
        [AWSSecret("MAP_USERNAME")]
        public string MapUserName { get; set; }
        /// <summary>
        /// Map password from AWS secrets
        /// </summary>
        [AWSSecret("MAP_PASSWORD")]
        public string MapPassword { get; set; }
        /// <summary>
        ///  AR checksum secret from AWS secrets
        /// </summary>
        [AWSSecret("AR_CHECKSUM_SECRET")]
        public string ArChecksumSecret { get; set; }
        /// <summary>
        /// AR system id from AWS secrets
        /// </summary>
        [AWSSecret("AR_SYSTEM_ID")]
        public string ArSystemId { get; set; }
        /// <summary>
        /// STS database name from AWS secrets
        /// </summary>
        [AWSSecret("DB_STS_NAME")]
        public string DbStsName { get; set; }
        /// <summary>
        /// STS database password from AWS secrets
        /// </summary>
        [AWSSecret("DB_STS_PASSWORD")]
        public string DbStsPassword { get; set; }
        /// <summary>
        /// STS database user name from AWS secrets
        /// </summary>
        [AWSSecret("DB_STS_USERNAME")]
        public string DbStsUserName { get; set; }
        /// <summary>
        /// Quality agent username from AWS secrets
        /// </summary>
        [AWSSecret("QA_USERNAME")]
        public string QaUserName { get; set; }
        /// <summary>
        /// Quality agent password from AWS secrets
        /// </summary>
        [AWSSecret("QA_PASSWORD")]
        public string QaPassword { get; set; }
        /// <summary>
        /// Email notification username from AWS secrets
        /// </summary>
        [AWSSecret("EN_USERNAME")]
        public string EnUserName { get; set; }
        /// <summary>
        /// Email notification password from AWS secrets
        /// </summary>
        [AWSSecret("EN_PASSWORD")]
        public string EnPassword { get; set; }
        /// <summary>
        /// Kapa CLS api key from AWS secrets
        /// </summary>
        [AWSSecret("KAPA_API_KEY")]
        public string KapaApiKey { get; set; }
        /// <summary>
        /// FINTO api key from AWS secrets
        /// </summary>
        [AWSSecret("FINTO_API_KEY")]
        public string FintoApiKey { get; set; }
        /// <summary>
        /// (Relaying) Translation order username from AWS secrets
        /// </summary>
        [AWSSecret("TRANS_ORDER_USERNAME")]
        public string TransOrderUsername { get; set; }
        /// <summary>
        /// (Relaying) Translation order password from AWS secrets
        /// </summary>
        [AWSSecret("TRANS_ORDER_PASSWORD")]
        public string TransOrderPassword { get; set; }
        /// <summary>
        /// Transifex authorization api key from AWS secrets
        /// </summary>
        [AWSSecret("TRANSIFEX_AUTHORIZATION")]
        public string TransifexAuthorization { get; set; }
        /// <summary>
        /// Access key for the IAM user accessing S3 storage.
        /// </summary>
        [AWSSecret("S3_ACCESS_KEY")]
        public string S3AccessKey { get; set; }
        /// <summary>
        /// Secret key for the IAM user accessing S3 storage.
        /// </summary>
        [AWSSecret("S3_SECRET_KEY")]
        public string S3SecretKey { get; set; }
        /// <summary>
        /// Access key for the IAM user accessing logs.
        /// </summary>
        [AWSSecret("LOG_ACCESS_KEY")]
        public string LogAccessKey { get; set; }
        /// <summary>
        /// Secret key for the IAM user accessing logs.
        /// </summary>
        [AWSSecret("LOG_SECRET_KEY")]
        public string LogSecretKey { get; set; }
        /// <summary>
        /// Secret key for the Y-platform token.
        /// </summary>
        [AWSSecret("Y_PLATFORM_TOKEN")]
        public string YPlatformToken { get; set; }
    }
}
