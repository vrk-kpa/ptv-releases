using System;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.Model.Models.Privileges
{
    internal class EntityBlockedAccessRight<T> : EntityBase, IEntityBlockedAccessRight where T : class, IEntityIdentifier
    {
        public Guid AccessBlockedId { get; set; }
        public AccessRightType AccessBlocked { get; set; }

        public Guid EntityId { get; set; }
        public T Entity { get; set; }
    }

    internal interface IEntityBlockedAccessRight
    {
        Guid AccessBlockedId { get; set; }
        AccessRightType AccessBlocked { get; set; }
    }
}