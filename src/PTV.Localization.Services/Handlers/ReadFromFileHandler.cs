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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PTV.Framework;
using PTV.Localization.Services.Model;

namespace PTV.Localization.Services.Handlers
{
    [RegisterService(typeof(ReadFromFileHandler), RegisterType.Transient)]
    [RegisterService(typeof(ITranslationDataHandler), RegisterType.Transient)]
    public class ReadFromFileHandler : TranslationDataHandlerBase
    {
        private string dedfaultFolder;
        private FileReader fileReader;
        
        public ReadFromFileHandler(IOptions<TransifexConfiguration> configuration, FileReader fileReader)
        {
            this.fileReader = fileReader;
            dedfaultFolder = configuration.Value.WorkingFolder;
        }

        public override string Key => "read";

        public string SuffixOptionName { get; set; } = "source";
        public string DataKey { get; set; }

        protected override ITranslationData ExecuteInternal(string languageCode, ITranslationData data, ITranslationOptions options)
        {
            fileReader.RunWithFile(() =>
            {
                string file = fileReader.GetFileName(fileReader.GetFolder(options), languageCode, GetSuffix(options));
                var translation = fileReader.ReadFile(file);
                data.SetData(languageCode, translation, DataKey);
            });
            return data;
        }

        private string GetSuffix(ITranslationOptions options)
        {
            return options.GetValue(SuffixOptionName);
        }
    }
}