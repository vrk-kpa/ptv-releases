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
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Framework;
using PTV.Localization.Services.Model;

namespace PTV.Localization.Services.Handlers
{
    [RegisterService(typeof(CompareDataHandler), RegisterType.Transient)]
    [RegisterService(typeof(ITranslationDataHandler), RegisterType.Transient)]
    public class CompareDataHandler : TranslationDataHandlerBase
    {
        public override string Key => "compare";
        
        public Func<string, string> GetCompareDataKey1 { get; set; } 
        public Func<string, string> GetCompareDataKey2 { get; set; }
        public bool CompareContent { get; set; }
        
        private bool IsDifferent { get; set; }

        public override ITranslationDataHandler Next => IsDifferent ? base.Next : null;
        protected override ITranslationData ExecuteInternal(string languageCode, ITranslationData data, ITranslationOptions options)
        {
            var data1 = data.GetData(GetCompareDataKey1?.Invoke(languageCode) ?? languageCode);
            var data2 = data.GetData(GetCompareDataKey2?.Invoke(languageCode) ?? languageCode);

            if (data1 != null && data2 != null)
            {
                Console.WriteLine($"Compare: {data1.Count} - {data2.Count}, compare content: {CompareContent}");
                if (data1.Count != data2.Count)
                {
                    IsDifferent = true;
                    return data;
                }
                if (CompareContent)
                {
                    foreach (var pair in data1)
                    {
                        if (data2.TryGet(pair.Key) != pair.Value)
                        {
                            IsDifferent = true;
                            return data;
                        }
                    }
                }
                else
                {
                    foreach (var key in data1.Keys)
                    {
                        if (!data2.ContainsKey(key))
                        {
                            IsDifferent = true;
                            return data;
                        }
                    }
                }
            }
            
            return data;
        }

        
    }
}