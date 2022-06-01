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

using System.Linq;
using PTV.Framework;
using PTV.Localization.Services.Model;

namespace PTV.Localization.Services.Handlers
{
//    [RegisterService(typeof(EmptyDataHandler), RegisterType.Transient)]
//    [RegisterService(typeof(ITranslationDataHandler), RegisterType.Transient)]
//    public class EmptyDataHandler : TranslationDataHandlerBase
//    {
//        public override string Key => "empty";
//
//        protected override ITranslationData ExecuteInternal(string languageCode, ITranslationData data, ITranslationOptions options)
//        {
//            var toUpdate = data.GetData(languageCode);
//
//            if (toUpdate != null)
//            {
//                var empty = toUpdate.Where(pair => string.IsNullOrEmpty(pair.Value) || pair.Key == pair.Value).ToDictionary(x => x.Key, x => x.Value);
//                data.SetData(languageCode, empty);
//            }
//            return data;
//        }
//    }
}
