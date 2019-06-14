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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Framework;
using PTV.Framework.ServiceManager;
using Remotion.Linq.Parsing;
using IOrganizationService = PTV.Database.DataAccess.Interfaces.Services.V2.IOrganizationService;

namespace PTV.NetworkServices.Emailing
{
    public class EmailGenerationSettings
    {
        public string AuthenticationEndPoint { get; set; }
        public string EmailEndPoint { get; set; }
        public string EmailTemplate { get; set; }
        public string EmailSentences { get; set; }
        public string AuthenticationUsername { get; set; }
        public string AuthenticationPassword { get; set; }
    }


    [RegisterService(typeof(IEmailService), RegisterType.Transient)]
    public class EmailService : IEmailService
    {
        private readonly Random randomGen = new Random();
        private readonly ProxyServerSettings proxySettings;
        private readonly INotificationService notificationService;
        private readonly ITasksService taskService;
        private readonly IOrganizationService organizationService;
        private const int retries = 5;
        private const string InsertPlace = "$INSERT$";
        
        public EmailService(IOptions<ProxyServerSettings> proxySettings, INotificationService notificationService, ITasksService taskService, IOrganizationService organizationService)
        {
            this.proxySettings = proxySettings.Value;
            this.notificationService = notificationService;
            this.taskService = taskService;
            this.organizationService = organizationService;
        }

        public void GenerateNotificationAndTaskEmails(EmailGenerationSettings emailGenerationSettings)
        {
            var allMainOrganizations = organizationService.GetMainRootOrganizationsIds();
            var sentences = emailGenerationSettings.EmailSentences.Replace("\r", string.Empty).Split('\n').Select(i => i.Split('|')).ToDictionary(i => i.First(), i => i.Last());
            var rowFormat = sentences["Row"];
            var token = GetPahaTokenAuthentication(emailGenerationSettings.AuthenticationEndPoint,
                new PahaAuthenticationData() {UserName = emailGenerationSettings.AuthenticationUsername, Password = emailGenerationSettings.AuthenticationPassword});
            if (string.IsNullOrEmpty(token)) return;
            var sahaMappings = organizationService.GetSahaIdsForPtvOrgRootIds(allMainOrganizations);
            //allMainOrganizations = new List<Guid>() { Guid.Parse("4d65575c-f052-4882-a24d-4356e74ca4ab")}; // sata test org
            foreach (var org in allMainOrganizations)
            {
                var notificationNumbers = notificationService.GetNotificationsNumbers(new List<Guid>() {org});
                var taskNumbers = taskService.GetTasksNumbers(new List<Guid>() {org});
                var notificationsLists = string.Join('\n',
                    notificationNumbers.Select(i =>
                        rowFormat.Replace(InsertPlace, sentences.TryGetOrDefault(i.Id.ToString(), i.Id.ToString()).Replace(InsertPlace, i.Count.ToString()))));

                var tasksLists = string.Join('\n',
                    taskNumbers.Select(
                        i => rowFormat.Replace(InsertPlace, sentences.TryGetOrDefault(i.Id.ToString(), i.Id.ToString()).Replace(InsertPlace, i.Count.ToString()))));

                if (!string.IsNullOrEmpty(notificationsLists) || !string.IsNullOrEmpty(tasksLists))
                {
                    if (string.IsNullOrEmpty(notificationsLists))
                    {
                        notificationsLists = sentences["NotificationsEmpty"];
                    }

                    if (string.IsNullOrEmpty(tasksLists))
                    {
                        tasksLists = sentences["TasksEmpty"];
                    }

                    var emailBody = emailGenerationSettings.EmailTemplate.Replace("$TASKS$", tasksLists).Replace("$NOTIFICATIONS$", notificationsLists);
                    SentEmailToOrganizationUsers(emailGenerationSettings.EmailEndPoint, token, sahaMappings[org], emailBody);
//                    if (!SentEmailToOrganizationUsers(emailGenerationSettings.EmailEndPoint, token, sahaMappings[org], emailBody))
//                    {
//                        break;
//                    }
                }
            }
        }

        public string GetPahaTokenAuthentication(string pahaAuthEndPoint, PahaAuthenticationData pahaAuthenticationData)
        {
            return Asyncs.HandleAsyncInSync(() => HttpClientWithProxy.UseAsync(proxySettings, async client =>
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                using (var stringContent = new StringContent(JsonConvert.SerializeObject(pahaAuthenticationData), Encoding.UTF8, MediaTypeNames.Application.Json))
                {
                    try
                    {
                        var response = await client.PostAsync(pahaAuthEndPoint, stringContent);
                        return JsonConvert.DeserializeObject<PahaAuthenticationToken>(await response.Content.ReadAsStringAsync()).AccessToken;
                    }
                    catch (Exception e) 
                    {
                        return string.Empty;
                    }
                }
            }, ignoreServerCertificate: true));
        }

        public bool SentEmailToOrganizationUsers(string emailEndPoint, string accessToken, Guid organizationId, string emailBody)
        {
            if (string.IsNullOrEmpty(accessToken) || !organizationId.IsAssigned() || string.IsNullOrEmpty(emailEndPoint) ||string.IsNullOrEmpty(emailBody))
            {
                return false;
            }
            
            var emailDefinition = new NotificationEmail();emailDefinition.Id = randomGen.Next();
            emailDefinition.AccountIds = new List<NotificationAccountId>() { new NotificationAccountId() { IdValue = organizationId }};
            emailDefinition.NotificationType = 1;
            emailDefinition.ServiceGroup = (int)(EPahaNotificationServiceGroup.Ptv | EPahaNotificationServiceGroup.Messages);
            emailDefinition.ServiceRoleGroup = 64; // ALLROLES = 64
            emailDefinition.PahaRoleGroup = 16; // ALLROLES = 16
            emailDefinition.SendToTarget = 2; // EMAIL = 2
            var emailContent = new NotificationText()
            {
                Message = emailBody.ToBase64(),
                Subject = "PTV Notification",
                Language = "fi",
                MessageFormat = "BASE64"
            };
            emailDefinition.NotificationText = new List<NotificationText>() {emailContent};
            int tries = 0;
            HttpResponseMessage response;
            do
            {
                response = Asyncs.HandleAsyncInSync(() => HttpClientWithProxy.UseAsync(proxySettings, async client =>
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    using (var stringContent = new StringContent(JsonConvert.SerializeObject(emailDefinition), Encoding.UTF8, MediaTypeNames.Application.Json))
                    {
                        try
                        {
                            Console.Write(stringContent.ReadAsStringAsync().Result);
                            return await client.PostAsync(emailEndPoint, stringContent);
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    }
                }, ignoreServerCertificate: true));
            } while (response?.IsSuccessStatusCode != true && (tries++ < retries));

            if (response == null)
            {
                throw new PtvAppException("Email service is not responding, terminated."); 
            }
            var pahaResponse = response.Content.ReadAsStringAsync().Result.DeserializeJsonObject<PahaNotificationServiceResponse>(new PahaNotificationServiceResponse() { HttpCode = 500 });
            return response.IsSuccessStatusCode && (pahaResponse.Status == 0 || pahaResponse.Status == 200) && (pahaResponse.Id == emailDefinition.Id);
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
        public string AccessToken { get; set; }
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