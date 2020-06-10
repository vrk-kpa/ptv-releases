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
using Microsoft.Extensions.CommandLineUtils;
using PTV.Framework;
using PTV.Localization.Services.Handlers;
using PTV.Localization.Services.Model;
using PTV.LocalizationTool.App.Handlers;

namespace PTV.LocalizationTool.App.Commands
{
    [RegisterService(typeof(ITranslationCommandFactory), RegisterType.Singleton)]
    internal class UploadCommand : CommandFactoryBase, ITranslationCommandFactory
    {
        private ITranslationDataHandler handler;

        public UploadCommand(CommandHandlerManager manager, CommandOptionHelper helper) : base(helper)
        {
            var dataKey = "Keys";
            handler = manager.Get<SingleFileReaderHandler>(r =>
            {
                r.DataKey = dataKey;
                r.SuffixOptionName = File;
            });
            handler
                .SetNext(manager.Get<TransifexDownloadHandler>())
                .SetNext(manager.Get<CompareDataHandler>(comp =>
                {
                    comp.GetCompareDataKey1 = lang => dataKey;
                    comp.GetCompareDataKey2 = lang => lang;
                }))
                .SetNext(manager.Get<TransifexUploadHandler>(upl =>
                    {
                        upl.DataKey = dataKey;
                        upl.Type = TransifexConfiguration.ParamTypes.Content.ToString().ToLower();
                    })
            );
        }

        public override string Name => "upload";

        private const string File = "file";

        protected override void SetupCommand(CommandLineApplication download)
        {
            download.Description = "Upload translation keys to server.";
            download.Option(
                $"-sf |--{File} <sourceKeysFiles>",
                "source file name with keys",
                CommandOptionType.SingleValue);
        }

        protected override ITranslationDataHandler GetHandler(CommandLineApplication command)
        {
            return handler;
        }
    }
}
