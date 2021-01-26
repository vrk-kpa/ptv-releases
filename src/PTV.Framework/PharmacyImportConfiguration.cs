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

using PTV.ExternalData.Pharmacy;

namespace PTV.Framework
{
    public class PharmacyImportConfiguration
    {
        public string DownloadUrl { get; set; }
        public string GetChannelUrl { get; set; }
        public string AuthenticationUrl { get; set; }
        public string PutLocationUrl { get; set; }
        public string PostLocationUrl { get; set; }
        public string PutEChannelUrl { get; set; }
        public string PostEChannelUrl { get; set; }
        public string PutConnectionUrl { get; set; }
        public string ApiUserName { get; set; }
        public string ApiPassword { get; set; }
        public string OrganizationId { get; set; }
        public string ServiceId { get; set; }
        public bool UsePaha { get; set; }

        public Secret GetSecret()
        {
            return new Secret
            (
                DownloadUrl,
                GetChannelUrl,
                AuthenticationUrl,
                PutLocationUrl,
                PostLocationUrl,
                PutEChannelUrl,
                PostEChannelUrl,
                PutConnectionUrl,
                ApiUserName,
                ApiPassword,
                OrganizationId,
                ServiceId,
                UsePaha
            );
        }
    }
}