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
using System.CodeDom;
using System.Collections.Generic;

namespace PTV.DataImport.WinExcelToJson.Model
{
    internal class LanguageLabel
    {
        public LanguageLabel(string label, string lang)
        {
            Label = label;
            Lang = lang;
        }

        public LanguageLabel()
        {
        }

        public string Label { get; set; }
        public string Lang { get; set; }
    }

    internal class LanguageName
    {
        public LanguageName(string name, string language)
        {
            Name = name;
            Language = language;
        }

        public LanguageName()
        {
        }

        public string Name { get; set; }
        public string Language { get; set; }
    }

    internal static class LanguageTextExtensions
    {
        public static void AddText(this List<LanguageLabel> list, string text, string language)
        {
            if (!string.IsNullOrEmpty(text))
            {
                list.Add(new LanguageLabel { Label = text, Lang = language});
            }
        }
    }
}
