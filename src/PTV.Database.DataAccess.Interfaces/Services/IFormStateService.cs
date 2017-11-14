using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using System;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IFormStateService
    {
        VmFormState Search(VmFormStateSearch search);
        string Exists(VmFormStateSearch search);
        VmFormState GetById(string id);
        VmFormState Save(VmFormState formState);
        void Delete(string id);
    }
}
