using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using PTV.Localization.Services.Model;

namespace PTV.Localization.Services.Helpers
{
    public class TransifexHelpers
    {
        public static Dictionary<string, string> ToLanguageDictionary(string raw)
        {
            var temp = JsonConvert.DeserializeObject<Dictionary<string, TransifexLanguageValue>>(raw);
            var result = new Dictionary<string, string>();
            foreach (var keyValuePair in temp)
            {
                // Convert Accessibility\\.ContactInfo\\.Title -> Accessibility.ContactInfo.Title 
                var key = keyValuePair.Key.Replace(@"\", string.Empty);
                result[key] = keyValuePair.Value.Value;
            }

            return result;
        }
        
        public static StartTranslationFileDownloadRequest CreateStartTranslationRequest(string language, string project, string resource)
        {
            return new StartTranslationFileDownloadRequest
            {
                Data = new ResouceTranslationRequestData
                {
                    Attributes = new Attributes { Mode = "onlytranslated"},
                    Relationships = new Relationships
                    {
                        Language = new IdAndTypeData
                        {
                            Data = new IdAndType { Id = $"l:{language}", Type = "languages"}
                        },
                        Resource = new IdAndTypeData
                        {
                            Data = new IdAndType
                            {
                                Id = $"o:population-register-center:p:{project}:r:{resource}",
                                Type = "resources"
                            }
                        }
                    }
                }
            };
        }

        public static string TryParseDownloadLink(string raw)
        {
            var data = JsonConvert.DeserializeObject<StartTranslationFileDownloadResponse>(raw);
            return data?.Data?.Links?.Self;
        }

        public static string GetHeadersSafely(HttpResponseMessage response)
        {
            try
            {
                var builder = new StringBuilder(1024);
                foreach (var header in response.Headers)
                {
                    var values = string.Join(",", header.Value.ToList());
                    builder.AppendLine($"{header.Key} : {values}");
                }

                return builder.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to iterate headers: {e}");
                return "Failed to iterate headers";
            }
        }
    }
}