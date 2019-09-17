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
using PTV.Framework;
using PTV.Localization.Services.Model;

namespace PTV.Localization.Services.Handlers
{
    [RegisterService(typeof(CopyDataHandler), RegisterType.Transient)]
    [RegisterService(typeof(ITranslationDataHandler), RegisterType.Transient)]
    public class CopyDataHandler : TranslationDataHandlerBase
    {
        public override string Key => "copy";
        
        public string DefaultsKey { get; set; } = "default";
        public string Keys { get; set; } = "keys";
        
        protected override ITranslationData ExecuteInternal(string languageCode, ITranslationData data, ITranslationOptions options)
        {
            var toUpdate = data.GetData(languageCode);

            var keys = data.GetData(Keys);
            var oldSource = GetDefaultTexts(languageCode, data, options);
            if (toUpdate == null || oldSource == null || keys == null)
            {
                return data;
            }

            foreach (KeyValuePair<string, string> pair in keys)
            {
                if (toUpdate.ContainsKey(pair.Key))
                {
                    if (string.IsNullOrEmpty(toUpdate[pair.Key]))
                    {
                        oldSource.TryGetValue(pair.Value, out var value);
                        if (value != null)
                        {
                            toUpdate[pair.Key] = value;
                        }
                        else
                        {
                            Console.Error.WriteLine($"Translation for language {languageCode} is missing for {pair.Key}: {pair.Value}.");
                        }
                    }
                }
                else
                {
                    throw new KeyNotFoundException($"Not found {pair.Key} - {pair.Value}.");
                }
            }
            return data;
        }

        private Dictionary<string, string> GetDefaultTexts(string languageCode, ITranslationData data, ITranslationOptions command)
        {
            return data.GetData(languageCode, DefaultsKey);
        }
    }
}