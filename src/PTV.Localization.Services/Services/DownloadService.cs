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

using System.Linq;
using PTV.Framework;
using PTV.Localization.Services.Handlers;
using PTV.Localization.Services.Model;

namespace PTV.Localization.Services.Services
{
    [RegisterService(typeof(IDownloadService), RegisterType.Transient)]
    internal class DownloadService : IDownloadService
    {
        private ITranslationDataHandler handler;

        static readonly string[] languages = {
            "fi", "sv", "en"
        };

        public DownloadService(TransifexDownloadHandler download, CleanEmptyDataHandler clean)
        {
            handler = download;
            download.SetNext(clean);
        }

        public ITranslationData Run()
        {
            ITranslationData data = new TranslationData();
            var translationOptions = new TranslationOptions();
            return languages.Aggregate(data, (current, language) => handler.Execute(language, current, translationOptions));
        }
    }

    public interface IDownloadService
    {
        ITranslationData Run();
    }

}
