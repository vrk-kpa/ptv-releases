using System.Collections.Generic;
using PTV.Database.Model.Models;

namespace PTV.Database.DataAccess.Interfaces.Caches.Finto
{
    internal interface IDigitalAuthorizationCacheInternal : IFintoCache<DigitalAuthorization>
    {
        public List<DigitalAuthorization> GetAsFintoTree();
    }
}
