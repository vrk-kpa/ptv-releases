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
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PTV.Localization.Services.Model
{
    public class TranslationData : ITranslationData
    {
        public Dictionary<string, ITranslationLanguageData> Translations { get; } = new Dictionary<string, ITranslationLanguageData>();
        
        public void SetData(string languageCode, string data, string key = "")
        {
            Translations[GetKey(languageCode, key)] = new TranslationLanguageData
            {
                Language = languageCode,
                Translation = JsonConvert.DeserializeObject<Dictionary<string, string>>(data)
            };
        }

        public void SetData(string languageCode, Dictionary<string, string> data, string key = "")
        {
            Translations[GetKey(languageCode, key)] = new TranslationLanguageData
            {
                Language = languageCode,
                Translation = data
            };
        }

        public Dictionary<string, string> GetData(string languageCode, string key = "")
        {
            if (Translations.TryGetValue(GetKey(languageCode, key), out ITranslationLanguageData data))
            {
                return data.Translation;
            }

            return null;
        }

        private string GetKey(string languageCode, string key = "")
        {
            return string.IsNullOrEmpty(key) ? languageCode : (languageCode + key);
        }
    }
}