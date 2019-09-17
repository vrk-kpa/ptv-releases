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
using System.Collections.Generic;
using System.Linq;
using PTV.Framework;
using PTV.Localization.Services.Model;

namespace PTV.Localization.Services.Handlers
{
    [RegisterService(typeof(UpdateWithDefaultDataHandler), RegisterType.Transient)]
    [RegisterService(typeof(ITranslationDataHandler), RegisterType.Transient)]
    public class UpdateWithDefaultDataHandler : TranslationDataHandlerBase
    {
        public override string Key => "update";

        public string DefaultsKey { get; set; } = "default";
        
        protected override ITranslationData ExecuteInternal(string languageCode, ITranslationData data, ITranslationOptions options)
        {
            var toUpdate = data.GetData(languageCode);

            var defTexts = GetDefaultTexts(languageCode, data, options);
            if (defTexts == null || toUpdate == null)
            {
                Console.WriteLine($"Command {Key} skipped, source data are empty.");
                return data;
            }

            var emptyText = toUpdate.Where(pair => string.IsNullOrEmpty(pair.Value) || pair.Key == pair.Value).ToList();
            foreach (KeyValuePair<string, string> pair in emptyText)
            {
                if (toUpdate.ContainsKey(pair.Key))
                {
                    defTexts.TryGetValue(pair.Key, out var value);
                    toUpdate[pair.Key] = value ?? string.Empty;
                }
            }
            return data;
        }

        private Dictionary<string, string> GetDefaultTexts(string languageCode, ITranslationData data, ITranslationOptions command)
        {
//            var defaultSuffix = helper.GetCommandOption(command, "default");
//            var file = fileReader.GetFileName(fileReader.GetFolder(command), languageCode, defaultSuffix);
//            data.SetData(languageCode + defaultSuffix, fileReader.ReadFile(file));
            return data.GetData(languageCode, DefaultsKey) ?? data.GetData(DefaultsKey);
        }
    }
}