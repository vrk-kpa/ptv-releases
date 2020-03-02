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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PTV.Framework;

namespace PTV.Localization.Services.Model
{
    public class TransifexConfiguration
    {
        public enum ParamTypes
        {
            Translation,
            Content
        }
        public string Project { get; set; }
        public string Url { get; set; }
        public Dictionary<string, TransifexProject> Projects { get; set; } = new Dictionary<string, TransifexProject>();
        public string Authorization { get; set; }
        public string WorkingFolder { get; set; }

        public Dictionary<string, List<string>> Params { get; set; }
        public ProxyServerSettings ProxyServerSettings { get; set; }

        public string GetUrl(string localization, string paramType)
        {
            var project = Projects.TryGet(Project) ?? throw new ArgumentException($"No projects found for {Project} in {string.Join(", ", Projects.Keys)}.");
            var parameters = Params.TryGetOrDefault(paramType ?? "translation", new List<string>());
            var paramValues = new Dictionary<string, string>
            {
                { "{project}", project.Name },
                { "{resource}", project.Resource },
                { "{localization}", localization }
            };
            return new Uri
            (
                new Uri(Url, UriKind.Absolute),
                Path.Combine(parameters.Select(x => paramValues.TryGetOrDefault(x, x)).ToArray())
            ).AbsoluteUri;
        }
    }
}
