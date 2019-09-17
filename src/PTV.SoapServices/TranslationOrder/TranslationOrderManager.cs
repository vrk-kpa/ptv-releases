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
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Services.Providers;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.TranslationOrder.Soap;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.ServiceManager;
using PTV.SoapServices.Enums;
using PTV.SoapServices.Interfaces;
using PTV.SoapServices.Interfaces.Lingsoft;
using PTV.SoapServices.Lingsoft;
using PTV.SoapServices.Models;
using PTV.ToolUtilities;
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
        private readonly ApplicationConfiguration configuration;
        private TranslationConfiguration translationConfiguration;
        private LingsoftClientManager lingsoftClientManager;
        
        public TranslationOrderManager(ITranslationService translationService,
                                       TranslationOrderServiceProvider translationOrderServiceProvider,
                                       ILogger<TranslationOrderManager> logger,
                                       ApplicationConfiguration configuration
                                      )
        {
            this.translationService = translationService;
            this.translationOrderServiceProvider = translationOrderServiceProvider;
            this.logger = logger;
            this.configuration = configuration;
        }

        public HealthCheckResult CallTestLingsoftConnection(TranslationConfiguration translationConfig)
        {
            this.translationConfiguration = translationConfig;
            if (string.IsNullOrEmpty(translationConfiguration.ServiceUrl))
            {
                return HealthCheckResult.NotConfigured;
            }
            using (var client = new HttpClient())
            {
                try
                {
                    var response = client.GetAsync(translationConfiguration.ServiceUrl).Result;
                    // 200 - 399 are OK
                    if ((int)response.StatusCode >= 400) return HealthCheckResult.Failed;
                    return !string.IsNullOrEmpty(response.Content?.ReadAsStringAsync()?.Result) ? HealthCheckResult.Ok : HealthCheckResult.Failed;
                }
                catch
                {
                    return HealthCheckResult.Failed;
                }
            }
        }
        
        
        private ILingsoftClientManager CreateLingsoftClient(TranslationConfiguration translationConfig)
        {
            BasicHttpBinding binding = new BasicHttpBinding(translationConfig.ServiceUrl.ToLowerInvariant().StartsWith("https") ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.TransportCredentialOnly);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            lingsoftClientManager = new LingsoftClientManager(translationConfig.ServiceUrl, configuration.GetEnvironmentType(), binding, translationConfig.UserName, translationConfig.Password, logger);
            var tempInfoMsg = $"***TR-ORDER INFO OrderManager*** translationConfig.ServiceUrl: {translationConfig.ServiceUrl} translationConfig.FileUrl:{translationConfig.FileUrl}";
            logger.LogInformation(tempInfoMsg);

            return lingsoftClientManager;
        }

        private VmOrderResponse ClientNewTranslationOrder(VmSoapTranslationOrder model, string fileUrl)
        {
            var response = new VmOrderResponse();
            try
            {
                using (var translationClient = CreateLingsoftClient(translationConfiguration))
                {
                    model.fileUrl = string.Format(fileUrl, model.Id); //Set endpoint
                    var orderData = SetOrderData(model);
                    response.TranslationCompanyOrderKey = translationClient.NewOrder(orderData)?.NewOrderResult;
                    var tempInfoMsg =
                        $"***TR-ORDER INFO OrderManager*** TranslationCompanyOrderKey: {response.TranslationCompanyOrderKey}";
                    logger.LogInformation(tempInfoMsg);
                }
            }
            catch (Exception e)
            {
                var errorMsg = $"TRANSLATION COMPANY ERROR: Translation order({model.Id}) method NewOrderAsync FAILURE - with exception:{e.GetType()} message:{e.Message}";
                response.ErrorMessage = errorMsg;
                logger.LogError(errorMsg);
            }

            return response;
        }
        
        private IOrder SetOrderData(VmSoapTranslationOrder model)
        {
            var newOrder = lingsoftClientManager.CreateOrder(model.TargetLanguageStateCultureCode);

            newOrder.name = $"{model.SourceOrganizationName} {model.SourceEntityName}";
            newOrder.orderID = Guid.NewGuid().ToString();
            newOrder.ordererAccount = model.SenderName;
            newOrder.email = model.SenderEmail;
            newOrder.srcLang = model.SourceLanguageStateCultureCode;
            newOrder.clientRef = model.ContentId.HasValue ? model.ContentId.ToString() : model.Id.ToString();
            newOrder.clientInfo = model.AdditionalInformation;
            newOrder.workType = (int) ClientWorkTypeEnum.Translation;
            newOrder.orderType = "ORDER";
            newOrder.filesUrl = model.fileUrl;
            newOrder.endCustomer = model.SourceOrganizationName;

            return newOrder;
        }

        private VmOrderState ClientGetOrderStatus(VmSoapTranslationOrder model)
        {
            IStatus response = null;
            VmOrderState result = new VmOrderState();
            try
            {
                using (var translationClient = CreateLingsoftClient(translationConfiguration))
                {
                    var orderStatusTask = translationClient.OrderStatus(model.TranslationCompanyOrderIdentifier);
                    response = orderStatusTask?.OrderStatusResult;
                }
            }
            catch (Exception e)
            {
                var errorMsg = $"TRANSLATION COMPANY ERROR: Translation order({model.Id}) method OrderStatus FAILURE - with exception:{e.GetType()} message:{e.Message}";
                result.ErrorMessage = errorMsg;
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

        private VmOrderResponse ClientUpdateTranslationOrder(VmSoapTranslationOrder model, string fileUrl)
        {
            var response = new VmOrderResponse();
            try
            {
                using (var translationClient = CreateLingsoftClient(translationConfiguration))
                {
                    model.fileUrl = string.Format(fileUrl, model.Id);
                    var updateOrderTask = translationClient.UpdateOrder(model.TranslationCompanyOrderIdentifier, SetOrderData(model));
                    var clientResponse = updateOrderTask.UpdateOrderResult;
                    response.Result = clientResponse == "1";
                }
            }
            catch(Exception e)
            {
                var errorMsg = $"TRANSLATION COMPANY ERROR: Translation order({model.Id}) method NewOrderAsync FAILURE - with exception:{e.GetType()} message:{e.Message} details:{CoreExtensions.ExtractAllInnerExceptions(e)}";
                response.ErrorMessage = errorMsg;
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

            var newOrders = translationService.GetTranslationOrdersByStateTypes(
                new List<TranslationStateTypeEnum>()
                {
                    TranslationStateTypeEnum.ReadyToSend
                });

            foreach (var newOrder in newOrders)
            {
                SendNewTranslationOrder(newOrder, translationConfiguration.FileUrl);
            }
        }

        private void SendNewTranslationOrder(VmSoapTranslationOrder model, string fileUrl)
        {
            var response = ClientNewTranslationOrder(model, fileUrl);
            var maxRepetitionErrorStatesNumber = translationConfiguration.MaxRepetitionErrorStatesNumber;

            if (!string.IsNullOrEmpty(response?.TranslationCompanyOrderKey))
            {
                try
                {
                    translationService.TranslationOrderSentSuccess(model.Id, response.TranslationCompanyOrderKey);
                    model.TranslationCompanyOrderIdentifier = response.TranslationCompanyOrderKey;
                    ProcessingOrderState(model);
                }
                catch (PtvTranslationException e)
                {
                    var errorMsg = $"TRANSLATION ERROR: Translation order({model.Id}) sending FAILURE - with exception:{e.GetType()} message:{e.Message} details:{CoreExtensions.ExtractAllInnerExceptions(e)}";
                    translationService.AddTranslationErrorStateWithRepetitionCheck(model.Id, maxRepetitionErrorStatesNumber, TranslationStateTypeEnum.SendError, errorMsg);
                    logger.LogError(errorMsg, e.StackTrace);
                }
            }
            else
            {
                var errorMsg = $"TRANSLATION ERROR: Translation order({model.Id}) sending FAILURE - empty order key generated from translation company with error message: '{response?.ErrorMessage}'";
                translationService.AddTranslationErrorStateWithRepetitionCheck(model.Id, maxRepetitionErrorStatesNumber, TranslationStateTypeEnum.SendError, errorMsg);
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
            var orders = translationService.GetTranslationOrdersByStateTypes(
                new List<TranslationStateTypeEnum>()
                {
                    TranslationStateTypeEnum.Sent,
                    TranslationStateTypeEnum.InProgress
                });

            foreach (var order in orders)
            {
                ProcessingOrderState(order);
            }
        }

        private async void ProcessingOrderState(VmSoapTranslationOrder model)
        {
            var resultClientState = ClientGetOrderStatus(model);
            var maxRepetitionErrorStatesNumber = translationConfiguration.MaxRepetitionErrorStatesNumber;
            logger.LogInformation($"Translation order({model.Id}) Soap response received, received state = {resultClientState.State}, PTV order state = {model.OrderStateType}");
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
                        translationService.AddTranslationErrorStateWithRepetitionCheck(model.Id, maxRepetitionErrorStatesNumber, TranslationStateTypeEnum.FileError);
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
                                var msg = $"Translation order({model.Id}) received in 'Arrived' state with FileURL: {resultClientState.FileUrl}";
                                translationService.AddTranslationOrderState(model.Id, TranslationStateTypeEnum.Arrived, msg);
                                logger.LogInformation(msg);
                            }
                            catch (PtvTranslationException e)
                            {
                                var errorMsg = $"TRANSLATION ERROR: Translation order({model.Id}) delivery process FAILURE - FileURL: {resultClientState.FileUrl} with exception:{e.GetType()} message:{e.Message} details:{CoreExtensions.ExtractAllInnerExceptions(e)}";
                                translationService.AddTranslationErrorStateWithRepetitionCheck(model.Id, maxRepetitionErrorStatesNumber, TranslationStateTypeEnum.DeliveredFileError, errorMsg);
                                logger.LogError(errorMsg, e.StackTrace);
                            }
                        }
                        else
                        {
                            var errorMsg = $"TRANSLATION ERROR: Translation order({model.Id}): Delivered FileUrl is null or empty";
                            translationService.AddTranslationErrorStateWithRepetitionCheck(model.Id, maxRepetitionErrorStatesNumber, TranslationStateTypeEnum.DeliveredFileError,errorMsg);
                            logger.LogError(errorMsg);
                        }
                    }
                    break;
                case Cancelled:
                    if (model.OrderStateType != TranslationStateTypeEnum.Canceled)
                    {
                        translationService.AddTranslationOrderState(model.Id, TranslationStateTypeEnum.Canceled);
                    }
                    break;
                case Not_Found:
                    {
                        var errorMsg = $"TRANSLATION ERROR: Translation order({model.Id}) recieved client state is: Not_Found (probably translation company error - Client.OrderStatus)" ;
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

            ProcessingTranslationOrdersWithError(sendErrorOrders);
        }

        private void ProcessingTranslationOrdersWithError(IReadOnlyList<VmSoapTranslationOrder> sendErrorOrders)
        {
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
                        SendNewTranslationOrder(order, translationConfiguration.FileUrl);
                    }
                    break;
                    case TranslationStateTypeEnum.FileError:
                    {
                        UpdateTranslationOrder(order, translationConfiguration.FileUrl); //resending order
                    }
                    break;
                    case TranslationStateTypeEnum.DeliveredFileError:
                    {
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
            var response = ClientUpdateTranslationOrder(model, fileUrl);

            if (!response.Result)
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
        
        private bool ClientCancelTranslationOrder(VmSoapTranslationOrder model)
        {
            var response = false;
            try
            {
                var translationClient = CreateLingsoftClient(translationConfiguration);
                var orderStatusTask = translationClient.CancelOrder(model.TranslationCompanyOrderIdentifier);
                response = orderStatusTask.CancelOrderResult;
            }
            catch (Exception e)
            {
                var errorMsg = $"TRANSLATION COMPANY ERROR: Translation order({model.Id}) method CancelOrder FAILURE - with exception:{e.GetType()} message:{e.Message}";
                logger.LogError(errorMsg);
            }

            return response;
        }
        
        
        private void CancelTranslationOrder(VmSoapTranslationOrder model)
        {
            var resultClientState = ClientGetOrderStatus(model);
            logger.LogInformation($"Cancel Translation order({model.Id}) ClientGetOrderStatus state = {resultClientState.State}, PTV order state = {model.OrderStateType}");

            switch (resultClientState.State)
            {
                case Queued:
                case File_error:
                        ClientCancelTranslationOrder(model);
                        translationService.AddTranslationOrderState(model.Id, TranslationStateTypeEnum.Canceled);
                    break;
                case In_progress: 
                case Delivered:
                case Cancelled:
                    if (model.OrderStateType != TranslationStateTypeEnum.Canceled)
                    {
                        translationService.AddTranslationOrderState(model.Id, TranslationStateTypeEnum.Canceled);
                    }
                    break;
                case Not_Found:
                {
                    if (model.Id.IsAssigned()) //ReadyToSent, SendErrror
                    {
                        translationService.AddTranslationOrderState(model.Id, TranslationStateTypeEnum.Canceled); 
                    }
                    
                    if (!string.IsNullOrEmpty(resultClientState.ErrorMessage))
                    {
                        var errorMsg = $"TRANSLATION ERROR: Cancelation Translation order({model.Id}) method OrderStatus FAILURE: with exception: {resultClientState.ErrorMessage})";
                        logger.LogError(errorMsg);
                    }
                }
                break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Cancel all translation orders in state request for cancel 
        /// </summary>
        /// <param name="translationConfig"></param>
        public void CancelTranslationOrders(TranslationConfiguration translationConfig)
        {
            this.translationConfiguration = translationConfig;
            
            var cancelOrderStates = translationService.GetTranslationOrdersByStateTypes(
                new List<TranslationStateTypeEnum>()
                {
                    TranslationStateTypeEnum.RequestForCancel
                });
            
            foreach (var order in cancelOrderStates)
            {
                CancelTranslationOrder(order); 
            }
        }
        
        /// <summary>
        /// Send all translation orders in state request for repetition
        /// </summary>
        /// <param name="translationConfig"></param>
        public void SendTranslationOrdersForRepetition(TranslationConfiguration translationConfig)
        {
            this.translationConfiguration = translationConfig;
            
            var requestForRepetitionOrderStates = translationService.GetTranslationOrdersByStateTypes(
                new List<TranslationStateTypeEnum>()
                {
                    TranslationStateTypeEnum.RequestForRepetition
                });
            
            translationService.CheckAndSetLastErrorState(requestForRepetitionOrderStates);
            
            ProcessingTranslationOrdersWithError(requestForRepetitionOrderStates);
        }
    }
}
