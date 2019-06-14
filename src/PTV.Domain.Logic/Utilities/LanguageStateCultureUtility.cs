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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PTV.Framework;

namespace PTV.Domain.Logic.Utilities
{
    [RegisterService(typeof(LanguageStateCultureUtility), RegisterType.Singleton)]
    public class LanguageStateCultureUtility
    {
        private readonly Dictionary<string, List<string>> allLanguagesStateCultures;

        public LanguageStateCultureUtility()
        {
            this.allLanguagesStateCultures = GetAllLanguagesStateCultures();
        }

        public string GetLanguageStateCulture(string language)
        {
            string result = null;
            if (allLanguagesStateCultures.ContainsKey(language))
            {
                var cultureList = allLanguagesStateCultures[language];

                switch (language)
                {
                    case "en":
                        result = language;
                        break;
                    case "fi":
                        result = cultureList.FirstOrDefault(x => x.EndsWith("FI"));
                        break;
                    case "sv":
                        result = cultureList.FirstOrDefault(x => x.EndsWith("SE"));
                        break;
                    default:
                        result = cultureList.FirstOrDefault();
                        break;
                }
            }

            return string.IsNullOrEmpty(result) ? language : result;
        }

        
        private Dictionary<string, List<string>> GetAllLanguagesStateCultures()
        {
            var result = new Dictionary<string, List<string>>();
            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (var culture in cultures)
            {
                var languageTag = string.IsNullOrEmpty(culture.Parent.IetfLanguageTag)
                    ? culture.IetfLanguageTag
                    : culture.Parent.IetfLanguageTag;

                if (string.IsNullOrEmpty(languageTag))
                {
                    continue;
                }

                if (result.ContainsKey(languageTag))
                {
                    var cultureList = result[languageTag];
                    cultureList.Add(culture.TextInfo.CultureName);
                }
                else
                {
                    result.Add(languageTag, new List<string>() { culture.TextInfo.CultureName });
                }
            }
            return result;
        }

    }
}