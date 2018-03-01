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
using System.Globalization;
using System.ServiceModel;
using Lingsoft;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Services.Providers;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.TranslationOrder.Soap;
using PTV.Framework;
using PTV.Framework.ServiceManager;
using PTV.SoapServices.Enums;
using PTV.SoapServices.Interfaces;
using PTV.SoapServices.Models;
using static PTV.SoapServices.Enums.ClientOrderStateEnum;

namespace PTV.SoapServices.TranslationOrder
{

    /// <summary>
    /// Translation Order Manager handle data from translation company
    /// </summary>
    [RegisterService(typeof(ITranslationOrderManager), RegisterType.Scope)]
    internal class TranslationOrderManager : ITranslationOrderManager
    {
        private readonly ITranslationService translationService;
        private readonly TranslationOrderServiceProvider translationOrderServiceProvider;
        private readonly ILogger logger;
        private TranslationConfiguration translationConfiguration;

        public TranslationOrderManager(ITranslationService translationService,
                                       TranslationOrderServiceProvider translationOrderServiceProvider,
                                       ILogger<TranslationOrderManager> logger)
        {
            this.translationService = translationService;
            this.translationOrderServiceProvider = translationOrderServiceProvider;
            this.logger = logger;
        }

        private LLSOrdersPortClient CreateLingsoftClient(TranslationConfiguration translationConfig)
        {
            BasicHttpBinding binding = new BasicHttpBinding(translationConfig.ServiceUrl.ToLowerInvariant().StartsWith("https") ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.TransportCredentialOnly);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            var lingsoftClient = new LLSOrdersPortClient(LLSOrdersPortClient.EndpointConfiguration.LLSOrdersPort, translationConfig.ServiceUrl);
            lingsoftClient.Endpoint.Binding = binding;
            lingsoftClient.Endpoint.EndpointBehaviors.Add(new FaultFormatingBehavior(logger));
            lingsoftClient.ClientCredentials.UserName.UserName = translationConfig.UserName;
            lingsoftClient.ClientCredentials.UserName.Password = translationConfig.Password;
            
            return lingsoftClient;
        }

        private string ClientNewTranslationOrder(VmSoapTranslationOrder model, string fileUrl)
        {
            string response = null;
            try
            {
                var translationClient = CreateLingsoftClient(translationConfiguration);
                model.fileUrl = string.Format(fileUrl, model.Id); //Set endpoint
                var orderData = SetOrderData(model);
                var newOrderTask = translationClient.NewOrderAsync(orderData);
                Console.WriteLine($"New order task:{newOrderTask}");
                newOrderTask.Wait();
                response = newOrderTask.Result?.NewOrderResult; 
                Console.WriteLine("New order response:", response);
            }
            catch (Exception e)
            {
                var errorMsg = $"TRANSLATION COMPANY ERROR: Method NewOrderAsync FAILURE - with exception:{e.GetType()} message:{e.Message}";
                Console.WriteLine(e);
                logger.LogError(errorMsg);
            }

            return response;
        }
        
        private Order SetOrderData(VmSoapTranslationOrder model)
        {
            //TODO Change
            var translateLanguages = new Dictionary<string, string>()
            {
                {"en", "en"},
                {"fi", "fi-FI"},
                {"sv", "sv-SE" }
            };

            var sourceLanguage = translateLanguages.ContainsKey(model.SourceLanguageCode) ? translateLanguages[model.SourceLanguageCode] : "fi-FI";
            var targetLanguage = translateLanguages.ContainsKey(model.TargetLanguageCode) ? translateLanguages[model.TargetLanguageCode] : "fi-FI";

            return new Order()
            {
                name = $"{model.SourceOrganizationName} {model.SourceEntityName}",
                orderID = Guid.NewGuid().ToString(),
                ordererAccount = model.SenderName,
                email = model.SenderEmail,
                srcLang = sourceLanguage,
                trgLangList = new trgLang[] { new trgLang() { trgLang1 = targetLanguage }},
                clientRef = model.ContentId.HasValue ? model.ContentId.ToString() : model.Id.ToString(),
                clientInfo = model.AdditionalInformation,
                workType = (int)ClientWorkTypeEnum.Translation,
                orderType = "ORDER",
                filesUrl = model.fileUrl,
                endCustomer = model.SourceOrganizationName
            };
        }

        private VmOrderState ClientGetOrderStatus(VmSoapTranslationOrder model)
        {
            Status response = null;
            VmOrderState result = new VmOrderState();
            try
            {
                var translationClient = CreateLingsoftClient(translationConfiguration);
                var orderStatusTask = translationClient.OrderStatusAsync(model.TranslationCompanyOrderIdentifier);
                orderStatusTask.Wait();
                response = orderStatusTask.Result?.OrderStatusResult;
            }
            catch (Exception e)
            {
                var errorMsg = $"TRANSLATION COMPANY ERROR: Method NewOrderAsync FAILURE - with exception:{e.GetType()} message:{e.Message}";
                Console.WriteLine(e);
                logger.LogError(errorMsg);
            }

            return response == null
                ? result
                : new VmOrderState()
                {
                    OrderId = response.orderID,
                    State = GetClientOrderStateEnum(response.state),
                    FileUrl = response.fileUrl,
                    Deadline = ParseDateTime(response.deadline), 
                    Contact = response.contact,
                    PhoneNumber = response.contact,
                    ValidationResult = response.validationResult
                };

            }

        private ClientOrderStateEnum GetClientOrderStateEnum(string state)
        {
            if (!string.IsNullOrEmpty(state) && Enum.TryParse(state, out ClientOrderStateEnum orderState))
            {
                return orderState;
            }
            return Not_Found;
        }

        private bool ClientUpdateTranslationOrder(VmSoapTranslationOrder model, string fileUrl)
        {
            var response = false;
            try
            {
                var translationClient = CreateLingsoftClient(translationConfiguration);
                model.fileUrl = string.Format(fileUrl, model.Id);
                var updateOrderTask =
                    translationClient.UpdateOrderAsync(model.TranslationCompanyOrderIdentifier, SetOrderData(model));
                updateOrderTask.Wait();
                var clientResponse = updateOrderTask.Result?.UpdateOrderResult;
                response = clientResponse == "1";
                Console.WriteLine("Update order response:", response);
            }
            catch(Exception e)
            {
                var errorMsg = $"TRANSLATION COMPANY ERROR: Method NewOrderAsync FAILURE - with exception:{e.GetType()} message:{e.Message}";
                Console.WriteLine(e);
                logger.LogError(errorMsg);
            }

            return response;
        }

        /// <summary>
        /// For sending all new translation orders to translation company
        /// </summary>
        /// <param name="translationConfig"></param>
        public void SendAllNewTranslationOrders(TranslationConfiguration translationConfig)
        {
            this.translationConfiguration = translationConfig;

            Console.WriteLine("Sending orders to translation company. {0}", DateTime.UtcNow); //Remove
            var newOrders = translationService.GetTranslationOrdersByStateTypes(
                new List<TranslationStateTypeEnum>()
                {
                    TranslationStateTypeEnum.ReadyToSend
                });

            Console.WriteLine("Number of translation orders: {0}", newOrders?.Count ?? 0); //Remove

            foreach (var newOrder in newOrders)
            {
                Console.WriteLine("Sending new translation order({0})", newOrder.Id); //Remove
                SendNewTranslationOrder(newOrder, translationConfiguration.FileUrl);
            }
        }

        private void SendNewTranslationOrder(VmSoapTranslationOrder model, string fileUrl)
        {
            var newOrderKey = ClientNewTranslationOrder(model, fileUrl);

            if (!string.IsNullOrEmpty(newOrderKey))
            {
                try
                {
                    translationService.TranslationOrderSentSuccess(model.Id, newOrderKey);
                    model.TranslationCompanyOrderIdentifier = newOrderKey;
                    ProcessingOrderState(model);
                    Console.WriteLine("Translation order({0}) were SUCCESFULLY send to Translation company.", model.Id); //Remove
                }
                catch (PtvTranslationException e)
                {
                    translationService.TranslationOrderSentFailure(model.Id);
                    var errorMsg = $"TRANSLATION ERROR: Translation order({model.Id}) sending FAILURE - with exception:{e.GetType()} message:{e.Message}"; 
                    Console.WriteLine(errorMsg); //Remove
                    logger.LogError(errorMsg, e.StackTrace);
                }
            }
            else 
            {
                translationService.TranslationOrderSentFailure(model.Id);
                var errorMsg = $"TRANSLATION ERROR: Translation order({model.Id}) sending FAILURE - empty order key generated from translation company"; 
                Console.WriteLine(errorMsg); //Remove
                logger.LogError(errorMsg);
            }
        }

        /// <summary>
        /// Processing states of translation order  
        /// </summary>
        /// <param name="translationConfig"></param>
        public void ProcessingTranslationOrderData(TranslationConfiguration translationConfig)
        {
            this.translationConfiguration = translationConfig;
            Console.WriteLine("Processing state data from Lingsoft {0}", DateTime.UtcNow); //Remove
            var orders = translationService.GetTranslationOrdersByStateTypes(
                new List<TranslationStateTypeEnum>()
                {
                    TranslationStateTypeEnum.Sent,
                    TranslationStateTypeEnum.InProgress
                });

            Console.WriteLine("Number of translation orders: {0}", orders?.Count ?? 0); //Remove
            foreach (var order in orders)
            {
                ProcessingOrderState(order);
            }
        }

        private async void ProcessingOrderState(VmSoapTranslationOrder model)
        {
            var resultClientState = ClientGetOrderStatus(model);
            logger.LogInformation($"Soap response received, received state = {resultClientState.State}, PTV order state = {model.OrderStateType}");
            switch (resultClientState.State)
            {
                case Queued:  //base
                    if (model.OrderStateType == TranslationStateTypeEnum.ReadyToSend || model.OrderStateType == TranslationStateTypeEnum.FileError || model.OrderStateType == TranslationStateTypeEnum.SendError)
                    {
                        translationService.AddTranslationOrderState(model.Id, TranslationStateTypeEnum.Sent);
                    }
                    break;
                case File_error:  //changed to file error
                    if (model.OrderStateType != TranslationStateTypeEnum.FileError)
                    {
                        translationService.AddTranslationOrderState(model.Id, TranslationStateTypeEnum.FileError);
                    }
                    break;
                case In_progress: //changed to in progress
                    if (model.OrderStateType != TranslationStateTypeEnum.InProgress)
                    {
                        translationService.AddTranslationOrderState(model.Id, TranslationStateTypeEnum.InProgress, deliverAt: resultClientState.Deadline);
                    }
                    break;
                case Delivered: //changed data 
                    if (model.OrderStateType != TranslationStateTypeEnum.Arrived)
                    {
                        if (!string.IsNullOrEmpty(resultClientState.FileUrl))
                        {
                            try
                            {
                                await translationOrderServiceProvider.DownloadTranslationOrderFile(resultClientState.FileUrl, model.Id, translationConfiguration?.ProxySettings);
                                translationService.AddTranslationOrderState(model.Id, TranslationStateTypeEnum.Arrived);
                                logger.LogInformation($"Translation order({model.Id}) received in 'Arrived' state with FileURL: {resultClientState.FileUrl}");
                                Console.WriteLine("Processing delivered translation."); //Remove
                            }
                            catch (PtvTranslationException e)
                            {
                                translationService.AddTranslationOrderState(model.Id, TranslationStateTypeEnum.DeliveredFileError);
                                var errorMsg =
                                    $"TRANSLATION ERROR: Translation order({model.Id}) delivery process FAILURE - FileURL: {resultClientState.FileUrl} with exception:{e.GetType()} message:{e.Message}";
                                if (e.InnerException != null)
                                {
                                    errorMsg += $"with inner exception:{e.InnerException.GetType()} message:{e.InnerException.Message}";
                                }
                                Console.WriteLine(errorMsg); //Remove
                                logger.LogError(errorMsg, e.StackTrace);
                            }
                        }
                        else
                        {
                            translationService.AddTranslationOrderState(model.Id, TranslationStateTypeEnum.DeliveredFileError);
                            var errorMsg = "TRANSLATION ERROR: Delivered FileUrl is null or empty";
                            Console.WriteLine(errorMsg); //Remove
                            logger.LogError(errorMsg);
                        }
                    }
                    break;
                    case Not_Found:
                    {
                        var errorMsg = "TRANSLATION ERROR: order state is: Not_Found";
                        Console.WriteLine(errorMsg);
                        logger.LogError(errorMsg);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Resending translation orders which are in error states
        /// </summary>
        /// <param name="translationConfig"></param>
        public void ResendTranslationOrdersWithError(TranslationConfiguration translationConfig)
        {
            this.translationConfiguration = translationConfig;

            var sendErrorOrders = translationService.GetTranslationOrdersByStateTypes(
                new List<TranslationStateTypeEnum>()
                {
                    TranslationStateTypeEnum.SendError,
                    TranslationStateTypeEnum.FileError,
                    TranslationStateTypeEnum.DeliveredFileError
                });

            foreach (var order in sendErrorOrders)
            {
                if (order.OrderStateType == null)
                {
                    continue;
                }

                switch (order.OrderStateType)
                {
                    case TranslationStateTypeEnum.SendError:
                    {
                        Console.WriteLine("Resending translation order({0}) of sending error - send new translation order", order.Id); 
                        SendNewTranslationOrder(order, translationConfiguration.FileUrl);
                    }
                    break;
                    case TranslationStateTypeEnum.FileError:
                    {
                        Console.WriteLine("Resending translation order({0}) of file error - update translation order", order.Id);
                        UpdateTranslationOrder(order, translationConfiguration.FileUrl); //resending order
                    }
                    break;
                    case TranslationStateTypeEnum.DeliveredFileError:
                    {
                        Console.WriteLine("Resending translation order({0}) of delivered file error - again download", order.Id);
                        ProcessingOrderState(order);  //new download translation
                    }
                        break;
                    default:
                        break;
                }
                }
        }

        private void UpdateTranslationOrder(VmSoapTranslationOrder model, string fileUrl)
        {
            var result = ClientUpdateTranslationOrder(model, fileUrl);

            if (!result)
            {
                logger.LogError($"TRANSLATION ERROR: Update operation of translation order({model.Id}) FAILED.");
            }

            ProcessingOrderState(model); //Get Status, change state to Send
        }

        private DateTime? ParseDateTime(string dateTime)
        {
            if (string.IsNullOrEmpty(dateTime))
            {
                return null;
            }

            DateTime dt;
            var result = DateTime.TryParse(dateTime,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dt);

            return result ? dt : (DateTime?)null;
        }
    }
}
