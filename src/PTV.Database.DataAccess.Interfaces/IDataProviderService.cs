using System;
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Interfaces
{
    public interface IDataProviderService
    {
        IReadOnlyList<IVmBase> Get(IUnitOfWork unitOfWork);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DataProviderServiceNameAttribute : Attribute
    {
        public string Name { get; private set; }

        public DataProviderServiceNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}
