using System;
using System.Collections.Generic;
using System.Text;
using PTV.Database.Model.Models;

namespace PTV.Database.DataAccess.Interfaces.Caches
{
    internal interface ICountryCache
    {
        Country GetByCode(string code);
        List<Country> GetAll();
    }
}
