using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Next.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Next.ServiceChannelConnection
{
    internal interface IEmailMapper
    {
        Dictionary<LanguageEnum, List<EmailLvModel>> MapEmails(ServiceServiceChannel connectionSource, 
            List<Guid> languageIds);
    }

    [RegisterService(typeof(IEmailMapper), RegisterType.Transient)]
    internal class EmailMapper: IEmailMapper
    {
        private readonly ILanguageCache languageCache;

        public EmailMapper(ILanguageCache languageCache)
        {
            this.languageCache = languageCache;
        }

        public Dictionary<LanguageEnum, List<EmailLvModel>> MapEmails(ServiceServiceChannel connectionSource, List<Guid> languageIds)
        {
            var result = new Dictionary<LanguageEnum, List<EmailLvModel>>();

            foreach(var languageId in languageIds)
            {
                var lang = Helpers.MapLanguage(languageCache, languageId);
                var emails = CreateEmails(connectionSource, languageId);
                if (emails.Any())
                {
                    result.Add(lang, emails);
                }
            }

            return result;
        }

        private List<EmailLvModel> CreateEmails(ServiceServiceChannel connectionSource, Guid languageId)
        {
            var result = new List<EmailLvModel>();
            var emails = connectionSource.ServiceServiceChannelEmails
                .Where(x => x.Email.LocalizationId == languageId)
                .OrderBy(x => x.Email.OrderNumber)
                .ToList();

            foreach(var email in emails)
            {
                result.Add(new EmailLvModel
                {
                    Id = email.Email.Id,
                    OrderNumber = email.Email.OrderNumber.HasValue ? email.Email.OrderNumber : 0,
                    Value = email.Email.Value.NullToEmpty(),
                    Description = email.Email.Description.NullToEmpty()
                });
            }

            return result;
        }
    }
}
