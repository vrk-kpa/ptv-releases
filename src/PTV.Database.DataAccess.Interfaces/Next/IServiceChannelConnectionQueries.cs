using PTV.Next.Model;
using System;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Interfaces.Next
{
    public interface IServiceChannelConnectionQueries
    {
        ConnectionModel GetConnectionForService(Guid serviceId, Guid serviceChannelUnificRootId);
        void RemoveConnection(Guid serviceId, Guid serviceChannelUnificRootId);
        List<ConnectableChannelModel> ConnectServiceToChannels(ConnectToChannelsModel model);
    }
}
