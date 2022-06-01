using System.Linq;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Services;
using PTV.Database.Model.Models;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next.Regions
{
    internal static class Mapper
    {
        internal static AreaModel ToModel(this Area input, ILanguageCache languageCache, IMunicipalityCache municipalityCache)
        {
            var municipalityIds = input.AreaMunicipalities.Select(x => x.MunicipalityId).ToHashSet();
            var municipalities = municipalityCache.GetAll().Where(x => municipalityIds.Contains(x.Id)).ToList();
            return new AreaModel()
            {
                Code = input.Code,
                Id = input.Id,
                Names = input.AreaNames
                    .Select(x => new
                        {Language = languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(), x.Name})
                    .ToDictionary(x => x.Language, x => x.Name),
                Municipalities = municipalities.Select(x => x.ToModel(languageCache)).ToList()
            };
        }
        
        internal static MunicipalityModel ToModel(this Municipality input, ILanguageCache languageCache)
        {
            return new MunicipalityModel()
            {
                Code = input.Code,
                Id = input.Id,
                PostalCodes = input.PostalCodes.Select(x => new PostalCodeModel
                {
                    Code = x.Code,
                    Names = x.PostalCodeNames
                        .Select(x => new
                            {Language = languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(), x.Name})
                        .ToDictionary(x => x.Language, x => x.Name)
                }).ToList(),
                Names = input.MunicipalityNames
                    .Select(x => new
                        {Language = languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(), x.Name})
                    .ToDictionary(x => x.Language, x => x.Name)
            };
        }
    }
}