using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Next.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Next.ServiceChannelConnection
{
    internal interface IWebPageMapper
    {
        Dictionary<LanguageEnum, List<WebPageLvModel>> MapWebPages(ServiceServiceChannel connectionSource, 
            List<Guid> languageIds);
    }

    [RegisterService(typeof(IWebPageMapper), RegisterType.Transient)]
    internal class WebPageMapper: IWebPageMapper
    {
        private readonly ILanguageCache languageCache;

        public WebPageMapper(ILanguageCache languageCache)
        {
            this.languageCache = languageCache;
        }

        public Dictionary<LanguageEnum, List<WebPageLvModel>> MapWebPages(ServiceServiceChannel connectionSource, 
            List<Guid> languageIds)
        {
            var result = new Dictionary<LanguageEnum, List<WebPageLvModel>>();

            foreach(var languageId in languageIds)
            {
                var lang = Helpers.MapLanguage(languageCache, languageId);
                var webPages = CreateWebPages(connectionSource, languageId);
                if (webPages.Any())
                {
                    result.Add(lang, webPages);
                }
            }

            return result;
        }

        private List<WebPageLvModel> CreateWebPages(ServiceServiceChannel connectionSource, Guid languageId)
        {
            var result = new List<WebPageLvModel>();            

            var webpages = connectionSource.ServiceServiceChannelWebPages
                .Where(x => x.LocalizationId == languageId)
                .OrderBy(x => x.OrderNumber)
                .ToList();

            foreach(var webpage in webpages)
            {
                result.Add(new WebPageLvModel
                {
                    Id = webpage.Id,
                    Name = webpage.Name.NullToEmpty(),
                    Url = webpage.WebPage.Url.NullToEmpty(),
                    AdditionalInformation = webpage.Description.NullToEmpty(),
                    OrderNumber = webpage.OrderNumber.HasValue ? webpage.OrderNumber : 0,
                });
            }

            return result;
        }
    }
}
