using System.Collections.Generic;

namespace PTV.Database.Model.Models.Privileges
{
    internal interface IBlockableEntity { }


    internal interface IBlockableEntity<T> : IBlockableEntity where T : IEntityBlockedAccessRight
    {
        ICollection<T> BlockedAccessRights { get; set; }
    }
}