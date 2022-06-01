using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using PTV.Framework;
using PTV.Localization.Services.Helpers;
using PTV.Localization.Services.Model;

namespace PTV.Localization.Services.Handlers
{
    [RegisterService(typeof(TransifexApiHandler), RegisterType.Transient)]
    public class TransifexApiHandler
    {
        private const string MediaType = "application/vnd.api+json";
        
        Dictionary<string, string> languageMapping = new Dictionary<string, string>
        {
            { "en", "en_GB"},
            { "fi", "fi_FI"},
            { "sv", "sv_SE"},
            { "af", "af"}
        };

        public TransifexApiHandler(IOptions<TransifexConfiguration> configuration)
        {
            Configuration = configuration.Value;
        }

        private TransifexConfiguration Configuration { get; set; }

        private string ToTransifexLanguage(string languageCode)
        {
            if (languageMapping.TryGetValue(languageCode, out string localization))
            {
                return localization;
            }
            throw new ArgumentOutOfRangeException($"Language code should be one of {string.Join(',', languageMapping.Values)}, but was {languageCode}");
        }
        
        public Dictionary<string, string> DownloadTranslations(string languageCode)
        {
            return StartAndPollTranslationCompletion(languageCode);
        }

        public string UploadTranslations(string languageCode, string content)
        {
            throw new NotImplementedException("Disabled temporarily because of transifex api upgrade");
        }

        public string ParamType { get; set; }

        private void SetHeader(HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Configuration.Authorization);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
        }
        
        private Dictionary<string, string> StartAndPollTranslationCompletion(string language)
        {
            var url = StartTranslationFileDownload(language);
            var fileLocationUrl = PollTranslationCompletion(url);
            if (fileLocationUrl == null)
            {
                throw new Exception($"Failed to download translations file for language {language}. Giving up.");
            }

            return DownloadTranslationsFile(fileLocationUrl);
        }

        private Uri PollTranslationCompletion(Uri url)
        {
            var policy = Policy
                .HandleResult<Uri>(uri => uri == null)
                .WaitAndRetry(8, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (result, sleepTime, context) =>
                    {
                        Console.WriteLine($"Waiting {sleepTime} before retrying");
                    });

            return policy.Execute(x => GetTranslationStatus(url), new Context("PollTranslationCompletion"));
        }
        
        private Uri GetTranslationStatus(Uri url)
        {
            return PtvHttpClient.UseWithoutAutoRedirect(client =>
            {
                SetHeader(client);
                Console.WriteLine($"Polling translation status from: {url}");
                var responseMessage = client.GetAsync(url).GetAwaiter().GetResult();
                var rawBody = responseMessage.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
                Console.WriteLine($"Translation status polling returned {responseMessage.StatusCode}");
                if (responseMessage.StatusCode == HttpStatusCode.SeeOther)
                {
                    return responseMessage.Headers.Location;
                }

                if (responseMessage.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    return null;
                }
                
                if (responseMessage.IsSuccessStatusCode && !string.IsNullOrEmpty(rawBody))
                {
                    var data = JsonConvert.DeserializeObject<TranslationStatusResponse>(rawBody);
                    var status = data?.Data?.Attributes?.Status;
                    Console.WriteLine($"Status of the translation job: {status}");
                    if (status == "pending" || status == "processing")
                    {
                        return null;
                    }
                }

                var headers = TransifexHelpers.GetHeadersSafely(responseMessage);
                throw new Exception($"Translation job has failed with StatusCode {responseMessage.StatusCode} raw body {rawBody} headers {headers}");
            });
        }
        
        private Dictionary<string, string> DownloadTranslationsFile(Uri url)
        {
            return PtvHttpClient.UseWithoutAutoRedirect(client =>
            {
                SetHeader(client);
                Console.WriteLine($"Trying to download translations file from: {url}");
                var responseMessage = client.GetAsync(url).GetAwaiter().GetResult();
                var raw = responseMessage.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
                Console.WriteLine($"Transifex returned {responseMessage.StatusCode}");
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    return TransifexHelpers.ToLanguageDictionary(raw);
                }

                var headers = TransifexHelpers.GetHeadersSafely(responseMessage);
                throw new Exception($"Failed to download translations file. StatusCode: {responseMessage.StatusCode} Response: {raw} Headers: {headers}");
            });
        }
        
        private Uri StartTranslationFileDownload(string language)
        {
            return PtvHttpClient.UseWithoutAutoRedirect(client =>
            {
                SetHeader(client);
                var request = CreateStartTranslationRequest(language);
                
                var data = JsonConvert.SerializeObject(request);
                var content = new StringContent(data, Encoding.UTF8);
                content.Headers.ContentType = new MediaTypeHeaderValue(MediaType);
                
                var url = $"{Configuration.Url}/resource_translations_async_downloads";
                Console.WriteLine($"Starting download translations file task by sending request to: {url}");
                var responseMessage = client.PostAsync(url, content).GetAwaiter().GetResult();
                return ParseDownloadLinkOrThrow(responseMessage);
            });
        }
        
        private Uri ParseDownloadLinkOrThrow(HttpResponseMessage responseMessage)
        {
            var raw = responseMessage.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            var headers = TransifexHelpers.GetHeadersSafely(responseMessage);
            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to start translations file download. Status code {responseMessage.StatusCode} response content {raw} headers {headers}");
            }

            // Documentation says that the link is in the Content-Location header but in QA it was in Location header
            // In production there was no location header so parsing from the actual response message
            var link = TransifexHelpers.TryParseDownloadLink(raw);
            if (string.IsNullOrEmpty(link))
            {
                throw new Exception($"Failed to start translation file download. Response does not contain link. Status code {responseMessage.StatusCode} response content {raw} headers {headers}");
            }
            
            return new Uri(link);
        }
        
        private StartTranslationFileDownloadRequest CreateStartTranslationRequest(string language)
        {
            var lang = ToTransifexLanguage(language);
            var project = GetProject();
            return TransifexHelpers.CreateStartTranslationRequest(lang, project.Name, project.Resource);
        }

        private TransifexProject GetProject()
        {
            if (Configuration.Projects.TryGetValue(Configuration.Project, out var project))
            {
                return project;
            }

            throw new Exception($"There is no configuration for project {Configuration.Project}");
        }
    }
}
