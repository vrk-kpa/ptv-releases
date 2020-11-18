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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using PTV.Framework;
using PTV.Localization.Services.Handlers;
using PTV.LocalizationTool.App.Handlers;

namespace PTV.LocalizationTool.App.Commands
{
    [RegisterService(typeof(ITranslationCommandFactory), RegisterType.Singleton)]
    internal class PipeCommand : CommandFactoryBase, ITranslationCommandFactory
    {
        private CommandHandlerManager commandHandlerManager;

        public PipeCommand(CommandHandlerManager manager, CommandOptionHelper helper) : base(helper)
        {
            commandHandlerManager = manager;
        }

        private Dictionary<char, (string name, string description)> valueOptions = new Dictionary<char, (string name, string description)>
        {
            {'r', ("read", "read from file (see command read)")},
            {'d', ("download", "download from transifex (see command download)")},
            {'c', ("clean", "clean empty texts (see command clean)")}
        };

        private CommandOption commands;

        public override string Name => "pipe";

        private string GetDescription()
        {
            return string.Join(
                Environment.NewLine,
                valueOptions.Select(x => $"{x.Key}: {x.Value.name} - {x.Value.description}")
            );
        }

        protected override void SetupCommand(CommandLineApplication updateDefault)
        {
            updateDefault.Description = "Update missing localization by default";
//            updateDefault.Argument("folder", "Working folder for source and output files");

            updateDefault.Option(
                "-s |--source <sourceSuffix>",
                "source file suffix",
                CommandOptionType.SingleValue);
            commands = updateDefault.Option(
                "-cmd |--commands <commandList>",
                $"chained commands: {GetDescription()}",
                CommandOptionType.SingleValue);
            updateDefault.Option(
                "-o |--output <outputSuffix>",
                "output file suffix",
                CommandOptionType.SingleValue);
//            updateDefault.OnExecute(() => Run(BuildHandlerChain(commands), updateDefault));
//            updateDefault.HelpOption("-? | -h | --help | -x");
        }

        protected override ITranslationDataHandler GetHandler(CommandLineApplication command)
        {
            return BuildHandlerChain(commands);
        }

        private ITranslationDataHandler BuildHandlerChain(CommandOption value)
        {
            Console.WriteLine($"{value.LongName}: {string.Join(" * ", value.Value().ToCharArray())} - {value.Value()}");

            List<ITranslationDataHandler> handlers = new List<ITranslationDataHandler>();
            foreach (char command in value.Value())
            {
                if (valueOptions.TryGetValue(command, out (string name, string description) definition))
                {
                    handlers.Add(commandHandlerManager.Get(definition.name));
                }
            }
            handlers.Add(commandHandlerManager.Get<TimeStampDataHandler>());
            handlers.Add(commandHandlerManager.Get("save"));
            handlers.Aggregate((previous, current) => previous.SetNext(current));
            return handlers.First();
        }
    }
}
