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
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Linq;
using Newtonsoft.Json;
using PTV.Framework;
using PTV.Localization.Services.Model;

namespace PTV.Localization.Services.Handlers
{
    [RegisterService(typeof(TransifexUploadHandler), RegisterType.Transient)]
    [RegisterService(typeof(ITranslationDataHandler), RegisterType.Transient)]
    public class TransifexUploadHandler : TranslationDataHandlerBase
    {
        private TransifexApiHandler apiHandler;

        public TransifexUploadHandler(TransifexApiHandler apiHandler)
        {
            this.apiHandler = apiHandler;
        }

        public override string Key => "upload";

        public string DataKey { get; set; }
        public string Type { get; set; }

        protected override ITranslationData ExecuteInternal(string languageCode, ITranslationData data, ITranslationOptions options)
        {
            if (options.GetValues("languages").Contains(languageCode) || !string.IsNullOrEmpty(DataKey))
            {
                var sourceKey = DataKey ?? languageCode;
                var content = data.GetData(sourceKey);
                if (content != null && data.GetData(sourceKey, "result") == null)
                {
                    var serializedData = JsonConvert.SerializeObject(content, Formatting.Indented);
                    apiHandler.ParamType = Type;
                    var result = apiHandler.UploadTranslations(languageCode, JsonConvert.SerializeObject(new {
                        content = serializedData
                    }));
                    data.SetData(sourceKey, result, "result");
                }
            }

            return data;
        }
    }
}
