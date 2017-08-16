using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    public class DownloadPostalCodesFromCodeServiceTask
    {
        private readonly IServiceProvider _serviceProvider;

        private const string PostalCodeServiceUrl = "https://cls-dev-public-api-service.kapa.ware.fi/cls-api/api/v1/postalcodes";

        private static readonly string PostalCodesGeneratedFile = Path.Combine("Generated", "PostalCode.json");

        public DownloadPostalCodesFromCodeServiceTask(IServiceProvider provider)
        {
            _serviceProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public void Download()
        {
            try
            {
                Console.WriteLine($"Starting download postal codes from: {PostalCodeServiceUrl}. This can take some time (roughly 1 minute).");
                Stopwatch sw = new Stopwatch();
                sw.Start();

                string postalCodesJson = null;

                using (var client = new HttpClient())
                {
                    // roughly 15MB
                    postalCodesJson = client.GetStringAsync(PostalCodeServiceUrl).Result;
                }

                sw.Stop();
                Console.WriteLine($"Postal codes downloaded in {sw.Elapsed}.");

                Console.WriteLine("Starting to parse results..");
                sw.Restart();
                JObject result = JObject.Parse(postalCodesJson);
                sw.Stop();
                Console.WriteLine($"Result parsed to JObject in {sw.Elapsed}");

                // read the meta info from the result
                int code = (int)result["meta"]["code"];
                

                if (code != 200)
                {
                    // something is wrong
                    Console.WriteLine($"Postal code service returned code: {code}. Something is wrong.");
                    return;
                }

                // this is just a quick and dirty solution so currently not deserializing to .net objects

                int totalResults = (int)result["meta"]["totalResults"];
                Console.WriteLine($"Code service returned {totalResults} postal code entries. Processing entries.");

                sw.Restart();
                JArray pcEntries = (JArray)result["results"];

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
                Console.WriteLine($"Entries processed in {sw.Elapsed}.");

                Console.WriteLine("Serializing to JSON and writing to a file.");
                sw.Restart();
                var json = JsonConvert.SerializeObject(listOfPostalCodes, Formatting.Indented);

                File.WriteAllText(PostalCodesGeneratedFile, json, Encoding.UTF8);

                sw.Stop();
                Console.WriteLine($"JSON serialization and file write took {sw.Elapsed}.");
                Console.WriteLine("Download postal codes from code service complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an error downloading and storing the postal codes from code service. {ex.ToString()}");
            }
        }
    }
}
