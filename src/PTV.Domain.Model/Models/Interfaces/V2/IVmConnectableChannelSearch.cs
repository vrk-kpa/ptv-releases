using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Domain.Model.Models.Interfaces.V2
{
    interface IVmConnectableChannelSearch
    {
        Guid? Id { get; set; }
        Guid? OrganizationId { get; set; }
        string ChannelType { get; set; }
    }
}
