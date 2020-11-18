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
using PTV.Framework;
using PTV.Localization.Services.Model;

namespace PTV.LocalizationTool.App.Handlers
{
    [RegisterService(typeof(CommandOptionHelper), RegisterType.Singleton)]
    internal class CommandOptionHelper
    {
        public string GetCommandOption(CommandLineApplication command, string name)
        {
            return command.Options.FirstOrDefault(x => x.ShortName == name || x.LongName == name || x.ValueName == name)?.Value();
        }

        public string GetCommandArgument(CommandLineApplication command, string name)
        {
            return command.Arguments.FirstOrDefault(x => x.Name == name)?.Value;
        }

        private object GetOptionValue(CommandOption x)
        {
            switch (x.OptionType)
            {
                case CommandOptionType.MultipleValue:
                    return x.Values;
                case CommandOptionType.NoValue:
                    return x.Value();
                default:
                    return x.Value();
            }
        }


        public ITranslationOptions BuildOptions(CommandLineApplication command, ITranslationOptions options = null)
        {
            var commandOptions = command.Options.Select(x => new KeyValuePair<string, object>(x.LongName, GetOptionValue(x)));
            if (options == null)
            {
                options = new TranslationOptions
                (
                    commandOptions
                );
            }
            else
            {
                options.AddRange(commandOptions);
            }
            command.Arguments.ForEach(x => options.Add(x.Name, x.Value) );
            return options;
        }
    }
}
