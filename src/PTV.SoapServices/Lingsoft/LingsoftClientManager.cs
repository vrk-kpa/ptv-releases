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
using PTV.SoapServices.Interfaces.Lingsoft;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.Extensions.Logging;
using PTV.Framework.Enums;
using PTV.SoapServices.TranslationOrder;

namespace PTV.SoapServices.Lingsoft
{
    public class LingsoftClientManager : ILingsoftClientManager
    {
        private ILingsoftClientManager LingsoftClient { get; set; }
        private readonly EnvironmentTypeEnum environmentType;

        public LingsoftClientManager(string serviceUrl, EnvironmentTypeEnum environmentType, BasicHttpBinding binding, string userName, string password, ILogger logger)
        {
            this.environmentType = environmentType;

            if ((environmentType & EnvironmentTypeEnum.Prod) != 0)
            {
                LingsoftClient = new LingsoftProd.LingsoftClient(LingsoftProd.LingsoftClient.EndpointConfiguration.LLSOrdersPort, serviceUrl);
            }
            else
            {
                LingsoftClient = new LingsoftTest.LingsoftClient(LingsoftTest.LingsoftClient.EndpointConfiguration.LLSOrdersPort, serviceUrl);
            }
            
            LingsoftClient.Endpoint.Binding = binding;
            LingsoftClient.Endpoint.EndpointBehaviors.Add(new FaultFormatingBehavior(logger));
            LingsoftClient.ClientCredentials.UserName.UserName = userName;
            LingsoftClient.ClientCredentials.UserName.Password = password;
        }
        
        public IOrderResponse NewOrder(IOrder order)
        {
            return (environmentType & EnvironmentTypeEnum.Prod) != 0
                ? LingsoftClient.NewOrder(order as LingsoftProd.Order)
                : LingsoftClient.NewOrder(order as LingsoftTest.Order);
        }
        
        public IUpdateOrderResponse UpdateOrder(string llsoWorkID, IOrder order)
        {
            return (environmentType & EnvironmentTypeEnum.Prod) != 0
                ? LingsoftClient.UpdateOrder(llsoWorkID, order as LingsoftProd.Order)
                : LingsoftClient.UpdateOrder(llsoWorkID, order as LingsoftTest.Order);
        }
        

        public IOrderStatusResponse OrderStatus(string llsoWorkID)
        {
            return LingsoftClient.OrderStatus(llsoWorkID);
        }
        
        public ICancelOrderResponse CancelOrder(string llsoWorkID)
        {
            return LingsoftClient.CancelOrder(llsoWorkID);
        }
        
        public IOrder CreateOrder(string lang)
        {
            if ((environmentType & EnvironmentTypeEnum.Prod) != 0)
            {
                return new LingsoftProd.Order()
                {
                    trgLangList = new LingsoftProd.trgLang[] { new LingsoftProd.trgLang() { trgLang1 = lang } }
                };
            }

            return new LingsoftTest.Order()
            {
                trgLangList = new LingsoftTest.trgLang[] { new LingsoftTest.trgLang() { trgLang1 = lang } }
            };
        }

        public void Dispose()
        {
            if (LingsoftClient?.InnerChannel == null) return;

            try
            {
                LingsoftClient.InnerChannel.Close();
            }
            catch (Exception)
            {
                LingsoftClient.InnerChannel.Abort();
            }
            
            LingsoftClient.InnerChannel.Dispose();
        }

        public ClientCredentials ClientCredentials { get; }
        public ServiceEndpoint Endpoint { get; }
        public CommunicationState State { get; }
        public IClientChannel InnerChannel { get; set; }
    }
}
