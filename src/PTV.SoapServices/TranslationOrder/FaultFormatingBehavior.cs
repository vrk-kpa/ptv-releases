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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace PTV.SoapServices.TranslationOrder
{
    internal class FaultFormatingBehavior : IEndpointBehavior
    {
        private readonly ILogger logger;

        public FaultFormatingBehavior(ILogger logger)
        {
            this.logger = logger;
        }

        public void Validate(ServiceEndpoint endpoint) { }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new FaultMessageInspector(logger));
        }
    }

    public class FaultMessageInspector : IClientMessageInspector
    {
        private readonly ILogger logger;

        public FaultMessageInspector(ILogger logger)
        {
            this.logger = logger;
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (reply.IsFault)
            {
                using (XmlDictionaryReader reader = reply.GetReaderAtBodyContents())
                {
                    var document = new XmlDocument();
                    document.Load(reader);
                    var nsmgr = new XmlNamespaceManager(document.NameTable);
                    var detailNode = document.SelectSingleNode("//faultstring", nsmgr);
                    var faultNode = document.SelectSingleNode("//faultcode", nsmgr);

                    var fc = new FaultCode(faultNode.InnerText);
                    var newFaultMessage = Message.CreateMessage(reply.Version, fc.Name, detailNode.InnerText);
                    reply = newFaultMessage;

                    if (reply != null)
                    {
                        var errorMessage = $"TRANSLATION COMPANY RESPONSE ERROR: with response message:{reply}";
                        logger.LogError(errorMessage);
                        throw new Exception(errorMessage);
                    }
                }
            }
        }
    }
}
