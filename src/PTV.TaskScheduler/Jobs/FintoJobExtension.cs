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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using PTV.Framework;
using PTV.TaskScheduler.Finto;

namespace PTV.TaskScheduler.Jobs
{
    internal static class FintoJobExtension
    {
        private static readonly string FintoSettingsFile = Path.Combine("Finto", "settings.json");

        internal static FintoSettings GetSettings(string fintoType, IWebHostEnvironment environment)
        {
            var settings = JsonConvert.DeserializeObject<FintoSettings>(File.ReadAllText(environment.GetFilePath("", FintoSettingsFile)));
            settings.Downloads = new List<string> { fintoType };
            return settings;
        }

        internal static FintoDownloadSettingsItem SetDownloadDefinition(FintoSettings settings, string fintoType)
        {
            if (!settings.DownloadDefinitions.ContainsKey(fintoType)) throw new Exception($"Type '{fintoType}' was not found in configuration file.");
            var typeSettings = settings.DownloadDefinitions[fintoType];
            typeSettings.MergeOnly = false;
            typeSettings.MergeTo = null;
            typeSettings.MergeFolder = null;
            typeSettings.DownloadFolder = null;
            typeSettings.FixHierarchy = false;
            typeSettings.SaveToFile = false;
            return typeSettings;
        }
    }
}
