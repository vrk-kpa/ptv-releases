using System;
using System.Collections.Generic;
using PTV.Database.Model.Models;

namespace PTV.Database.DataAccess.Interfaces.Caches.Finto
{
    internal interface IOntologyTermDataCacheInternal : IFintoCache<OntologyTerm>
    {
        List<OntologyTerm> GetOntologyTermsForExactMatches(IEnumerable<string> uris);
        DateTime GetLastUpdate();
        IEnumerable<OntologyTerm> GetById(IEnumerable<Guid> ids);
        List<OntologyTerm> SearchOntologyTermByName(string name, string orderLanguageCode,
            List<Guid> languagesIds = null);
    }
}
