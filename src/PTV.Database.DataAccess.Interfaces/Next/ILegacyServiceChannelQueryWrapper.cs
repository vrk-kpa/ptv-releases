using PTV.Next.Model;
using System;

namespace PTV.Database.DataAccess.Interfaces.Next
{
    public interface ILegacyServiceChannelQueryWrapper
    {
        ConnectableChannelSearchResultModel GetConnectableChannels(ConnectableChannelSearchModel searchParameters);        
        ServiceChannelModel GetChannelLatestVersionByUnificRootId(Guid unificRootId, bool includeConnections);
    }
}
