using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Domain.Model.Models.Interfaces.V2
{
    interface IVmConnectableServiceSearch
    {
        Guid? OrganizationId { get; set; }
        string Name { get; set; }
    }
}
