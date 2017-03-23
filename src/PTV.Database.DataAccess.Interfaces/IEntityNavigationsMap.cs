using System;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Interfaces
{
    public interface IEntityNavigationsMap
    {
        Dictionary<Type, EntityPropertiesDefinition> NavigationsMap { get; }
    }
}