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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PTV.DataImport.Console.Tasks
{
    public class DownloadPostalCodesFromCodeServiceTask
    {
        private const string PostalCodeServiceUrl = "https://cls-dev-public-api-service.kapa.ware.fi/cls-api/api/v1/postalcodes";

        private static readonly string PostalCodesGeneratedFile = Path.Combine("Generated", "PostalCode.json");

        public void Download()
        {
            try
            {
                System.Console.WriteLine($"Starting download postal codes from: {PostalCodeServiceUrl}. This can take some time (roughly 1 minute).");
                var sw = new Stopwatch();
                sw.Start();

                string postalCodesJson;

                using (var client = new HttpClient())
                {
                    // roughly 15MB
                    postalCodesJson = client.GetStringAsync(PostalCodeServiceUrl).Result;
                }

                sw.Stop();
                System.Console.WriteLine($"Postal codes downloaded in {sw.Elapsed}.");

                System.Console.WriteLine("Starting to parse results..");
                sw.Restart();
                var result = JObject.Parse(postalCodesJson);
                sw.Stop();
                System.Console.WriteLine($"Result parsed to JObject in {sw.Elapsed}");

                // read the meta info from the result
                var code = (int)result["meta"]["code"];


                if (code != 200)
                {
                    // something is wrong
                    System.Console.WriteLine($"Postal code service returned code: {code}. Something is wrong.");
                    return;
                }

                // this is just a quick and dirty solution so currently not deserializing to .net objects

                var totalResults = (int)result["meta"]["totalResults"];
                System.Console.WriteLine($"Code service returned {totalResults} postal code entries. Processing entries.");

                sw.Restart();
                var pcEntries = (JArray)result["results"];

                var listOfPostalCodes = pcEntries.Select(e => new {
                    Code = (string)e["code"],
                    Type = (string)e["typeCode"],
                    MunicipalityCode = (string)e["municipality"]["code"],
                    Names = new[] {
                        new { Language = "fi", Name = (string)e["names"]["fi"] },
                        new { Language = "sv", Name = (string)e["names"]["se"] }
                    }
                });

                sw.Stop();
                System.Console.WriteLine($"Entries processed in {sw.Elapsed}.");

                System.Console.WriteLine("Serializing to JSON and writing to a file.");
                sw.Restart();
                var json = JsonConvert.SerializeObject(listOfPostalCodes, Formatting.Indented);

                File.WriteAllText(PostalCodesGeneratedFile, json, Encoding.UTF8);

                sw.Stop();
                System.Console.WriteLine($"JSON serialization and file write took {sw.Elapsed}.");
                System.Console.WriteLine("Download postal codes from code service complete.");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"There was an error downloading and storing the postal codes from code service. {ex}");
            }
        }
    }
}
