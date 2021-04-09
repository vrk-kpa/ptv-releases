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

namespace PTV.Framework
{
    // https://hkp.maanmittauslaitos.fi/hkp/published/fi/c494438d-7503-465f-bd83-44973cbb973d
    // https://hkp.maanmittauslaitos.fi/hkp/published/sv/05bca0f8-79fa-42e6-8a9b-e1ef6fbf9b0b
    // https://hkp.maanmittauslaitos.fi/hkp/published/en/5a536374-be0f-4971-8e5f-5763360b1a40
    // http://ptvenv.cloudapp.net

    // https://hkp.maanmittauslaitos.fi/hkp/published/fi/659e0e69-d447-4930-b4c8-6c2ea4848fd5
    // https://hkp.maanmittauslaitos.fi/hkp/published/sv/bf9440f5-76c9-44e1-8bcd-eb728e364774
    // https://hkp.maanmittauslaitos.fi/hkp/published/en/ba3b3b5f-ca5c-4747-90d9-accc991baa3b
    // https://palvelutietovaranto.qa.suomi.fi

    // https://hkp.maanmittauslaitos.fi/hkp/published/fi/c84a44a2-60a7-4612-ad4e-e067c0729e35
    // https://hkp.maanmittauslaitos.fi/hkp/published/sv/431e67de-0405-415c-acbe-2d41611d86fa
    // https://hkp.maanmittauslaitos.fi/hkp/published/en/b5868b8c-05fe-45fe-8a35-321a813c0d3b
    // https://palvelutietovaranto.suomi.fi/

    // https://hkp.maanmittauslaitos.fi/hkp/published/fi/0acccad0-e347-4ca9-a1f1-883898f1e17b
    // https://hkp.maanmittauslaitos.fi/hkp/published/sv/6fc1118f-f45f-4ae8-bf43-946ebc60782f
    // https://hkp.maanmittauslaitos.fi/hkp/published/en/fe880ce6-9581-4966-b613-0f2f47c1805b
    // https://palvelutietovaranto.trn.suomi.fi

    /// <summary>
    /// Contains ionformation map dns
    /// </summary>
    public class MapDNSes
    {
        private string _fi = string.Empty;

        /// <summary>
        /// Fi DNS
        /// </summary>
        public string Fi
        {
            get { return Uri.EscapeUriString(_fi); }
            set { _fi = value; }
        }

        private string _sv = string.Empty;

        /// <summary>
        /// Sv DNS
        /// </summary>
        public string Sv
        {
            get { return Uri.EscapeUriString(_sv); }
            set { _sv = value; }
        }

        private string _en = string.Empty;
        /// <summary>
        /// En DNS
        /// </summary>
        public string En
        {
            get { return Uri.EscapeUriString(_en); }
            set { _en = value; }
        }
    }

    public class RequestFilterAppSetting
    {
        public int MaxRequestsPerIp { get; set; }
        public int MaxRequestsPerUser { get; set; }
        public int MaxRequestsTotal { get; set; }
        public int MaxAnonymousRequestsPerIp { get; set; }
    }

}
