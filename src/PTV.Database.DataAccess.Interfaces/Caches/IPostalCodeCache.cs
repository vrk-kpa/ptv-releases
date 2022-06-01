using PTV.Database.Model.Models;
using System;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Interfaces.Caches
{
    internal interface IPostalCodeCache
    {
        Guid GuidByCode(string code);
        Guid? MunicipalityIdForCode(string code);
        Guid? MunicipalityIdForPostalId(Guid id);
        List<PostalCode> GetAll();
        PostalCode GetById(Guid postalCodeId);
    }
}
