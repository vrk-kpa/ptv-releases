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
using System;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using PTV.Framework;
using PTV.Localization.Services.Handlers;
using PTV.LocalizationTool.App.Handlers;

namespace PTV.LocalizationTool.App.Commands
{
    [RegisterService(typeof(ITranslationCommandFactory), RegisterType.Singleton)]
    internal class UpdateCommand : CommandFactoryBase, ITranslationCommandFactory
    {
        private ITranslationDataHandler handler;

        public UpdateCommand(CommandHandlerManager manager, CommandOptionHelper helper) : base(helper)
        {
            handler = manager.Get("read");
            handler
                .SetNext(manager.Get<ReadFromFileHandler>(h =>
                {
                    h.DataKey = "default";
                    h.SuffixOptionName = "default";
                }))
                .SetNext(manager.Get<UpdateWithDefaultDataHandler>(h => { h.DefaultsKey = "default"; }))
                .SetNext(manager.Get("save"));
        }

        public override string Name => "update";

        protected override void SetupCommand(CommandLineApplication updateDefault)
        {
            updateDefault.Description = "Update missing localization by default";
//            updateDefault.Argument("folder", "Working folder for source and output files");

            updateDefault.Option(
                "-s |--source <sourceSuffix>",
                "source file suffix",
                CommandOptionType.SingleValue);
            updateDefault.Option(
                "-d |--default <defaultJsonFile>",
                "default texts file suffix",
                CommandOptionType.SingleValue);
            updateDefault.Option(
                "-o |--output <outputSuffix>",
                "output file suffix",
                CommandOptionType.SingleValue);
//            updateDefault.OnExecute(() => Run(handler, updateDefault));
//            updateDefault.HelpOption("-? | -h | --help | -x");
        }

        protected override ITranslationDataHandler GetHandler(CommandLineApplication command)
        {
            return handler;
        }
    }
}
