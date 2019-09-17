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
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using PTV.Framework;
using PTV.Localization.Services.Handlers;
using PTV.LocalizationTool.App.Handlers;

namespace PTV.LocalizationTool.App.Commands
{
    [RegisterService(typeof(ITranslationCommandFactory), RegisterType.Singleton)]
    internal class DefaultForEmptyCommand : CommandFactoryBase, ITranslationCommandFactory
    {
        private ITranslationDataHandler handler;
        public override string Name => "default";
        
        public DefaultForEmptyCommand(CommandHandlerManager manager, CommandOptionHelper helper) : base(helper)
        {
            handler = manager.Get("read");
            handler
                .SetNext(manager.Get<SingleFileReaderHandler>(h => 
                { 
                    h.SuffixOptionName = "default";
                    h.DataKey = "default";
                }))
                .SetNext(manager.Get("clean"))
                .SetNext(manager.Get("update"))
                .SetNext(manager.Get("save"));
        }

        protected override void SetupCommand(CommandLineApplication command)
        {
            command.Description = "Fill empty keys with default to single file.";
//            command.Argument("folder", "Working folder for source and output files");
            command.Option(
                "-s |--source <sourceSuffix>",
                "source file suffix",
                CommandOptionType.SingleValue);
            command.Option(
                "-o |--output <outputSuffix>",
                "output file suffix",
                CommandOptionType.SingleValue);
            command.Option(
                "-d |--default <defaultJsonFile>",
                "default texts file suffix",
                CommandOptionType.SingleValue);
//            command.OnExecute(() => Run(handler, command));
//            command.HelpOption("-? | -h | --help | -x");
        }

        protected override ITranslationDataHandler GetHandler(CommandLineApplication command)
        {
            return handler;
        }
    }
}