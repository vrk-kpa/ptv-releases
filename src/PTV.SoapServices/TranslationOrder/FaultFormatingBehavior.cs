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
                    logger.LogError($"TRANSLATION COMPANY ERROR: Response - {reply}");
                    reply = newFaultMessage;
                }
            }
        }
    }
}
