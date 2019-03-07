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
using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;
using PTV.Framework;

namespace PTV.LocalizationTool.App.Handlers
{
    [RegisterService(typeof(CopyDataHandler), RegisterType.Transient)]
    [RegisterService(typeof(ITranslationDataHandler), RegisterType.Transient)]
    internal class CopyDataHandler : TranslationDataHandlerBase
    {
        private FileReader fileReader;
        private CommandOptionHelper helper;

        public CopyDataHandler(FileReader read, CommandOptionHelper helper)
        {
            fileReader = read;
            this.helper = helper;
        }

        public override string Key => "copy";
        
        public string DefaultsKey { get; set; } = "default";
        public string Keys { get; set; } = "keys";
        
        protected override ITranslationData ExecuteInternal(string languageCode, ITranslationData data, CommandLineApplication command)
        {
            var toUpdate = data.GetData(languageCode);

            var keys = GetKeys(data, command);
            var oldSource = GetDefaultTexts(languageCode, data, command);

            foreach (KeyValuePair<string, string> pair in keys)
            {
                if (toUpdate.ContainsKey(pair.Key))
                {
                    if (string.IsNullOrEmpty(toUpdate[pair.Key]))
                    {
                        oldSource.TryGetValue(pair.Value, out var value);
                        if (value != null)
                        {
                            toUpdate[pair.Key] = value;
                        }
                        else
                        {
                            Console.Error.WriteLine($"Translation for language {languageCode} is missing for {pair.Key}: {pair.Value}.");
                        }
                    }
                }
                else
                {
                    throw new Exception($"Not found {pair.Key} - {pair.Value}.");
                }
            }
            return data;
        }

        private Dictionary<string, string> GetDefaultTexts(string languageCode, ITranslationData data, CommandLineApplication command)
        {
//            var defaultSuffix = helper.GetCommandOption(command, "default");
//            var file = fileReader.GetFileName(fileReader.GetFolder(command), languageCode, defaultSuffix);
//            data.SetData(languageCode + defaultSuffix, fileReader.ReadFile(file));
            return data.GetData(languageCode, DefaultsKey);
        }
        
        private Dictionary<string, string> GetKeys(ITranslationData data, CommandLineApplication command)
        {
            var fileName = helper.GetCommandOption(command, "keys");
            var keys = data.GetData(fileName);
            if (keys != null)
            {
                return keys;
            }
            
            var file = fileReader.GetFileName(fileReader.GetFolder(command), fileName);

            data.SetData(fileName, fileReader.ReadFile(file));
            return data.GetData(fileName);
        }
    }
}