using PTV.Next.Model;
using PTV.Framework;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Database.DataAccess.Interfaces.DbContext;

namespace PTV.Database.DataAccess.Next.ServiceChannelConnection
{
    [RegisterService(typeof(IServiceChannelConnectionCommands), RegisterType.Transient)]
    internal class ServicChannelConnectionCommands : IServiceChannelConnectionCommands
    {
        private readonly IConnectionsService connectionService;
        private readonly ISaveConnectionMapper mapper;
        private readonly IServiceService serviceService;
        private readonly IConnectionAccessCheck accessCheck;

        public ServicChannelConnectionCommands(IConnectionsService connectionService,
            ISaveConnectionMapper mapper,
            IServiceService serviceService,
            IConnectionAccessCheck accessCheck)
        {
            this.connectionService = connectionService;
            this.mapper = mapper;
            this.serviceService = serviceService;
            this.accessCheck = accessCheck;
        }

        public void SaveConnection(ConnectionModel model)
        {
            // Old layer wants to save the whole connection list so we need
            // to load the service with all the connections and map them into
            // correct model
            var serviceSearchParams = new VmServiceBasic
            {
                Id = model.ServiceId,
                IncludeConnections = true
            };

            var service = serviceService.GetService(serviceSearchParams);

            accessCheck.CanModifyConnection(service.Id, service.PublishingStatus, service.Organization);

            var oldModel = mapper.Map(model, service.Connections);
            connectionService.SaveServiceRelations(oldModel);
        }
    }
}