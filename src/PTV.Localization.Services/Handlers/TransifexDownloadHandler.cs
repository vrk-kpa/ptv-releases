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

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PTV.Framework;
using PTV.Localization.Services.Model;

namespace PTV.Localization.Services.Handlers
{
    [RegisterService(typeof(TransifexDownloadHandler), RegisterType.Transient)]
    [RegisterService(typeof(ITranslationDataHandler), RegisterType.Transient)]
    public class TransifexDownloadHandler : TranslationDataHandlerBase
    {
        private TransifexApiHandler apiHandler;
//        private string authorization;

        public TransifexDownloadHandler(TransifexApiHandler apiHandler)
        {
            this.apiHandler = apiHandler;
//            authorization = configuration["authorization"];
        }

        public override string Key => "download";

        protected override ITranslationData ExecuteInternal(string languageCode, ITranslationData data,
            ITranslationOptions options)
        {
            string result = apiHandler.DownloadTranslations(languageCode);
            if (!string.IsNullOrEmpty(result))
            {
                data.SetData(languageCode, JsonConvert.DeserializeObject<TransifexData>(result).Content);
            }

            return data;
        }
    }
}
