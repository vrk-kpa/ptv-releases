using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using System;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IFormStateService
    {
        VmFormState Search(VmFormStateSearch search);
        VmFormState GetById(Guid id);
        VmFormState Save(VmFormState formState);
        void Delete(Guid id);
        void Delete(Guid entityId, string userName);
    }
}
