using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PTV.Database.Model.Models;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Interfaces.Next
{
    public interface ITranslationQueries
    {
        TranslationDetailModel GetDetails(Guid id);
        List<TranslationHistoryModel> GetServiceHistory(Guid serviceId, LanguageEnum languageEnum);
    }
}