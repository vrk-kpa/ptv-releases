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
using Microsoft.Extensions.CommandLineUtils;
using PTV.Localization.Services.Handlers;
using PTV.Localization.Services.Model;
using PTV.LocalizationTool.App.Handlers;

namespace PTV.LocalizationTool.App.Commands
{
    internal abstract class CommandFactoryBase : ITranslationCommandFactory
    {
        private CommandOptionHelper helper;
        protected CommandFactoryBase(CommandOptionHelper helper)
        {
            this.helper = helper;
        }

        readonly string[] languages = {
            "fi", "sv", "en", "af"
        };

        private CommandLineApplication baseCommand;

        protected virtual IEnumerable<string> Languages => languages;

        private IEnumerable<string> GetFilteredLanguages(IEnumerable<string> filteredLanguages)
        {
            return filteredLanguages.Any() ?
                Languages.Intersect(filteredLanguages) :
                Languages;
        }

        public abstract string Name { get; }

        public Action<CommandLineApplication> Create(CommandLineApplication commandLineApplication)
        {
            return command =>
            {
                baseCommand = commandLineApplication;
                SetupCommand(command);
                command.OnExecute(() => Run(GetHandler(command), command));
                command.HelpOption("-? | -h | --help | -x");
            };
        }
        
        protected abstract void SetupCommand(CommandLineApplication command);

        protected abstract  ITranslationDataHandler GetHandler(CommandLineApplication command);
        
        protected int Run(ITranslationDataHandler handler, CommandLineApplication command)
        {
            Console.WriteLine($"{command.Name}: {command.Description}");
            ITranslationData data = new TranslationData();
            var options = helper.BuildOptions(baseCommand);
            helper.BuildOptions(command, options);
            var languagesFilter = options.GetValues("languages");
            foreach (var language in GetFilteredLanguages(languagesFilter))
            {
                Console.WriteLine($"run for {language}");
                data = handler.Execute(language, data, options);
            }
            return 0;
        }
    }
}