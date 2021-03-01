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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Logging;
using PTV.Framework.ServiceManager;
using IOrganizationService = PTV.Database.DataAccess.Interfaces.Services.V2.IOrganizationService;

namespace PTV.NetworkServices.Emailing
{
    public class EmailGenerationSettings
    {
        public string AuthenticationEndPoint { get; set; }
        public string EmailEndPoint { get; set; }
        public string EmailTemplate { get; set; }
        public string EmailSubject { get; set; }
        public Dictionary<string,string> EmailSentences { get; set; }
        public string AuthenticationUsername { get; set; }
        public string AuthenticationPassword { get; set; }
    }


    [RegisterService(typeof(IEmailService), RegisterType.Transient)]
    public class EmailService : IEmailService
    {
        private readonly Random randomGen = new Random();
        private readonly INotificationService notificationService;
        private readonly ITasksService taskService;
        private readonly IOrganizationService organizationService;
        private readonly ILogger<EmailService> jobLogger;
        private readonly IOrganizationTreeDataCache organizationCache;
        private readonly ILanguageCache languageCache;
        private const int Retries = 5;
        private const string InsertPlace = "$INSERT$";
        private const string DatePlace = "$DATE$";
        private const string FinnishOrgName = "$ORGFI$";
        private const string SwedishOrgName = "$ORGSW$";
        private readonly List<TasksIdsEnum> filterTasks = new List<TasksIdsEnum> { TasksIdsEnum.ExceptionLinks }; 

        public EmailService(
            INotificationService notificationService,
            ITasksService taskService,
            IOrganizationService organizationService,
            ILogger<EmailService> jobLogger,
            IOrganizationTreeDataCache organizationCache,
            ILanguageCache languageCache)
        {
            this.notificationService = notificationService;
            this.taskService = taskService;
            this.organizationService = organizationService;
            this.jobLogger = jobLogger;
            this.organizationCache = organizationCache;
            this.languageCache = languageCache;
        }

        public void GenerateNotificationAndTaskEmails(EmailGenerationSettings emailGenerationSettings, VmJobLogEntry loggerInfo)
        {
            var allMainOrganizations = organizationService.GetMainRootOrganizationsIds(new List<PublishingStatus> { PublishingStatus.Published, PublishingStatus.Draft, PublishingStatus.Modified });

            var sentences = emailGenerationSettings.EmailSentences.ToDictionary(i => i.Key,
                s => s.Value.Replace("\r", string.Empty).Split('\n').Select(i => i.Split('|')).ToDictionary(i => i.First(), i => i.Last()));

            var token = GetPahaTokenAuthentication(emailGenerationSettings.AuthenticationEndPoint,
                emailGenerationSettings.AuthenticationUsername, emailGenerationSettings.AuthenticationPassword, loggerInfo);
            if (string.IsNullOrEmpty(token))
            {
                jobLogger.LogSchedulerError(loggerInfo, "Email notification failed on authentication. Empty token.");
                return;
            }

            var sahaMappings = organizationService.GetSahaIdsForPtvOrgRootIds(allMainOrganizations);
            var counter = 0;
            foreach (var org in allMainOrganizations)
            {
                jobLogger.LogSchedulerInfo(loggerInfo, $"Org number: {counter++} Org count: {allMainOrganizations.Count.ToString()}, Org id: {org.ToString()}");
                var organizationTree = organizationCache.GetAllSubOrganizationIds(org);
                var notificationNumbers = notificationService.GetNotificationsNumbers(organizationTree);
                var taskNumbers = taskService.GetTasksNumbers(organizationTree).Where(x => !filterTasks.Contains(x.Id)).ToList() ;
                if (notificationNumbers.All(i => i.Count == 0) && taskNumbers.All(i => i.Count == 0)) continue;
                var dateTime = DateTime.UtcNow;
                var emailBody = emailGenerationSettings.EmailTemplate.Replace(DatePlace, dateTime.Month+"/"+dateTime.Year);
                var emailSubject = emailGenerationSettings.EmailSubject.Replace(DatePlace, dateTime.Month+"/"+dateTime.Year);
                emailBody = FillOrganizationNames(emailBody, org);
                bool shouldBeSent = false;

                foreach (var lang in sentences)
                {
                    var langSentences = lang.Value;
                    var rowFormat = langSentences["Row"];

                    var notificationsLists = string.Join('\n',
                        notificationNumbers.Where(i => i.Count > 0).Select(i =>
                            rowFormat.Replace(InsertPlace, langSentences.TryGetOrDefault(i.Id.ToString(), i.Id.ToString()).Replace(InsertPlace, i.Count.ToString()))));

                    var tasksLists = string.Join('\n',
                        taskNumbers.Where(i => i.Count > 0).Select(
                            i => rowFormat.Replace(InsertPlace, langSentences.TryGetOrDefault(i.Id.ToString(), i.Id.ToString()).Replace(InsertPlace, i.Count.ToString()))));

                    if (string.IsNullOrEmpty(notificationsLists))
                    {
                        notificationsLists = rowFormat.Replace(InsertPlace, langSentences["NotificationsEmpty"]);
                    }
                    else
                    {
                        shouldBeSent = true;
                    }

                    if (string.IsNullOrEmpty(tasksLists))
                    {
                        tasksLists = rowFormat.Replace(InsertPlace, langSentences["TasksEmpty"]);
                    }
                    else
                    {
                        shouldBeSent = true;
                    }

                    emailBody = emailBody.Replace($"$TASKS_{lang.Key}$", tasksLists).Replace($"$NOTIFICATIONS_{lang.Key}$", notificationsLists);
                }

                if (shouldBeSent)
                {
                    foreach (var targetOrgId in sahaMappings[org])
                    {
                        SendEmailToOrganizationUsers(emailGenerationSettings.EmailEndPoint, token, targetOrgId, emailBody, emailSubject, loggerInfo);
                    }
                }
            }
        }

        private string FillOrganizationNames(string emailTemplate, Guid organizationId)
        {
            var finnish = languageCache.Get("fi");
            var swedish = languageCache.Get("sw");
            var finnishName = organizationCache.GetName(organizationId, finnish);
            var swedishName = organizationCache.GetName(organizationId, swedish);
            return emailTemplate.Replace(FinnishOrgName, finnishName).Replace(SwedishOrgName, swedishName);
        }


        public string GetPahaTokenAuthentication(string pahaAuthEndPoint, string username, string password, VmJobLogEntry loggerInfo)
        {
            var pahaAuthenticationData = new PahaAuthenticationData{ Password = password, UserName =  username };
            jobLogger.LogSchedulerInfo(loggerInfo, $"Get PAHA token authentication start. UserName: { pahaAuthenticationData.UserName }, URL: { pahaAuthEndPoint }");
            return Asyncs.HandleAsyncInSync(() => PtvHttpClient.UseAsync(async client =>
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                using (var stringContent = new StringContent(JsonConvert.SerializeObject(pahaAuthenticationData), Encoding.UTF8, MediaTypeNames.Application.Json))
                {
                    try
                    {
                        using (var response = await client.PostAsync(pahaAuthEndPoint, stringContent))
                        {
                            jobLogger.LogSchedulerInfo(loggerInfo, $"Received response. Status code: { response?.StatusCode }");

                            if (response?.IsSuccessStatusCode != true)
                            {
                                throw new Exception($"No success response from service, HTTP status code {response}");
                            }

                            var token = await response.Content.ReadAsStringAsync();
                            return JsonConvert.DeserializeObject<PahaAuthenticationToken>(token).ServiceToken;
                        }
                    }
                    catch (Exception e)
                    {
                        jobLogger.LogSchedulerError(loggerInfo, "Email notification failed on authentication endpoint.", e);
                        return string.Empty;
                    }
                }
            }, ignoreServerCertificate: true));
        }

        public (bool success, string pahaMessage) SendEmailToOrganizationUsers(string emailEndPoint, string accessToken,
            Guid organizationId, string emailBody, string emailSubject, VmJobLogEntry loggerInfo)
        {
            if (string.IsNullOrEmpty(accessToken) || !organizationId.IsAssigned() ||
                string.IsNullOrEmpty(emailEndPoint) || string.IsNullOrEmpty(emailBody))
            {
                return (false, null);
            }

            var emailDefinition = new NotificationEmail();
            emailDefinition.Id = randomGen.Next();
            emailDefinition.AccountIds = new List<NotificationAccountId>
                {new NotificationAccountId {IdValue = organizationId}};
            emailDefinition.NotificationType = 1;
            emailDefinition.ServiceGroup =
                (int) (EPahaNotificationServiceGroup.Ptv | EPahaNotificationServiceGroup.Messages | EPahaNotificationServiceGroup.EIdentification);
            emailDefinition.ServiceRoleGroup = 3; // ADMIN&USER==3
            emailDefinition.PahaRoleGroup = 16; // ALLROLES = 16
            emailDefinition.SendToTarget = 2; // EMAIL = 2
            emailDefinition.SendToHtml = true;
            emailDefinition.InsertHeaderFooter = false;
            var emailContent = new NotificationText
            {
                Message = emailBody.ToBase64(),
                Subject = emailSubject,
                Language = "fi",
                MessageFormat = "BASE64",
                SendToHtml = true,
                InsertHeaderFooter = false
            };
            emailDefinition.NotificationText = new List<NotificationText> {emailContent};
            int tries = 0;
            string responseContent;
            bool responseIsSuccessStatus = false;
            var responseStatusCode = -1;
            do
            {
                (responseIsSuccessStatus, responseStatusCode, responseContent) = Asyncs.HandleAsyncInSync(() =>
                    PtvHttpClient.UseAsync(async client =>
                    {

                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", accessToken);
                        using (var stringContent = new StringContent(JsonConvert.SerializeObject(emailDefinition),
                            Encoding.UTF8, MediaTypeNames.Application.Json))
                        {
                            try
                            {
                                using (var response = await client.PostAsync(emailEndPoint, stringContent))
                                {
                                    var success = response?.IsSuccessStatusCode ?? false;
                                    var content = response?.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                                    if (success)
                                    {
                                        jobLogger.LogSchedulerInfo(loggerInfo, "Email was successfully sent.");
                                    }

                                    return (success, (int?) response?.StatusCode ?? -1, content);
                                }
                            }
                            catch (Exception e)
                            {
                                jobLogger.LogSchedulerError(loggerInfo,
                                    $"Sending of email notification failed. OrgId {organizationId}.", e);
                            }
                        }

                        return (false, -1, string.Empty);
                    }, ignoreServerCertificate: true));
            } while (!responseIsSuccessStatus && (tries++ < Retries));

            if (string.IsNullOrEmpty(responseContent) || (!responseIsSuccessStatus))
            {
                jobLogger.LogSchedulerError(loggerInfo,
                    $"Sending of email notification failed. No response from service. Status {responseStatusCode}, content: {responseContent}.");

                // Timeout, no need to try sending other emails.
                if (responseStatusCode < 0)
                {
                    throw new PtvAppException($"Email service is not responding, terminated.");
                }
            }

            var pahaResponse = responseContent.DeserializeJsonObject(new PahaNotificationServiceResponse {HttpCode = 500});
            jobLogger.LogSchedulerInfo(loggerInfo,
                $"SentEmailToOrganizationUsers result pahaResponse.Status:{pahaResponse.Status} pahaResponse.message: {pahaResponse.Message} pahaResponse.timestamp: {pahaResponse.Timestamp} pahaResponse.Id == emailDefinition.Id:{pahaResponse.Id == emailDefinition.Id}");
            return (
                responseIsSuccessStatus && (pahaResponse.Status == 0 || pahaResponse.Status == 200) &&
                (pahaResponse.Id == emailDefinition.Id), pahaResponse.Message);
        }
    }

    public class PahaAuthenticationData
    {
        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }
    }

    public class PahaAuthenticationToken
    {
        public string ServiceToken { get; set; }
    }

    public class PahaNotificationServiceResponse
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "httpCode")]
        public int HttpCode { get; set; }

        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "system")]
        public string System { get; set; }

        [JsonProperty(PropertyName = "errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "requestId")]
        public string RequestId { get; set; }

        [JsonProperty(PropertyName = "moreInfo")]
        public string MoreInfo { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty(PropertyName = "field")]
        public string Field { get; set; }
    }
}
