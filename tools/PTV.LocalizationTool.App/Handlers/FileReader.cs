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
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using PTV.Framework;

namespace PTV.LocalizationTool.App.Handlers
{
    [RegisterService(typeof(FileReader), RegisterType.Transient)]
    internal class FileReader
    {
        private string defaultFolder;
        private CommandOptionHelper helper;

        public FileReader(IConfigurationRoot configuration, CommandOptionHelper helper)
        {
            defaultFolder = configuration["WorkingFolder"];
            this.helper = helper;
        }

        public void RunWithFile(Action action)
        {
            try
            {
                action();
            }
            catch (FileNotFoundException e)
            {
                Console.Error.WriteLine($"File {e.FileName} not found.");
            }
        }
        
        public string GetFileName(string folder, string fileName, string suffix = null, string prefix = null, FileNameCreateType type = FileNameCreateType.Simple)
        {
            var composedFileName = string.Join("_", new[] {prefix, fileName, suffix}.WhereNotNull());
            if ((type & FileNameCreateType.LogFileName) > 0)
            {
                Console.WriteLine($"{composedFileName}.");
            }
            return Path.Combine(folder, $"{composedFileName}.json");
        }

        public string ReadFile(string fullPath)
        {
            return File.ReadAllText(fullPath);
        }

        public string GetFolder(CommandLineApplication command)
        {
            return helper.GetCommandArgument(command, "folder") ?? defaultFolder;
        }
    }
}