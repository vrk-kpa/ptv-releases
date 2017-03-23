using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;

namespace PTV.Database.Model.ServiceDataHolders
{
    internal class OrganizationTreeItem
    {
        public ICollection<OrganizationTreeItem> Children { get; set; }
        public OrganizationTreeItem Parent { get; set; }
        public OrganizationVersioned Organization { get; set; }
        public Guid UnificRootId { get; set; }
    }
}
