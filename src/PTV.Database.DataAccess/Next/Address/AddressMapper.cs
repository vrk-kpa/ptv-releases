using System.Collections.Generic;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Next.Model;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Next.Street
{
    public interface IAddressMapper
    {
        StreetSearchResult Map(VmSearchResult<VmStreet> source);
        StreetNameResult Map(List<VmLanguageText> source);
        StreetModel Map(VmStreet source);
    }

    [RegisterService(typeof(IAddressMapper), RegisterType.Transient)]
    public class AddressMapper: IAddressMapper
    {
        private readonly ILanguageCache languageCache;
        
        public AddressMapper(ILanguageCache languageCache)
        {
            this.languageCache = languageCache;
        }
        
        public StreetSearchResult Map(VmSearchResult<VmStreet> source)
        {
            var result = new StreetSearchResult
            {
                MaxPageCount = source.MaxPageCount,
                PageNumber = source.PageNumber,
                MoreAvailable = source.MoreAvailable,
                Skip = source.Skip,
                Items = source.SearchResult.Select(Map).ToList()
            };
            return result;
        }

        public StreetModel Map(VmStreet source)
        {
            var result = new StreetModel
            {
                Id = source.Id,
                IsValid = source.IsValid,
                MunicipalityCode = source.MunicipalityCode,
                Names = source.Translation != null ? source.Translation.Texts
                    .Select(x => new {Language = x.Key.ToEnum<LanguageEnum>(), x.Value})
                    .ToDictionary(x => x.Language, x => x.Value) : new Dictionary<LanguageEnum, string>(),
                StreetNumbers = source.StreetNumbers != null ? source.StreetNumbers.Select(Map).ToList() : new List<StreetNumberModel>()
            };
            return result;
        }

        private StreetNumberModel Map(VmStreetNumber source)
        {
            var result = new StreetNumberModel
            {
                StartNumber = source.StartNumber,
                EndNumber = source.EndNumber,
                IsEven = source.IsEven,
                PostalCode = source.PostalCode.Code,
                IsValid = source.IsValid,
                Id = source.Id,
                StreetId = source.StreetId
            };
            return result;
        }

        public StreetNameResult Map(List<VmLanguageText> source)
        {
            return new StreetNameResult()
            {
                Items = source.Select(x => new Dictionary<LanguageEnum, string>
                {
                    {this.languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(), x.Text}
                }).ToList()
            };
        }
    }
}
