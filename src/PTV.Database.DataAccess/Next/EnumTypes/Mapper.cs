using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next.EnumTypes
{
    internal static class Mapper
    {
        internal static OntologyModel MapSingle(this OntologyTerm input, ILanguageCache languageCache)
        {
            if (input == null || languageCache == null)
            {
                return null;
            }
            
            return new OntologyModel
            {
                Code = input.Code,
                Descriptions = new Dictionary<LanguageEnum, string>(),
                Id = input.Id,
                Names = input.Names
                    .Select(y => new { Language = languageCache.GetByValue(y.LocalizationId).ToEnum<LanguageEnum>()
                        , y.Name} )
                    .ToDictionary(y => y.Language, y => y.Name)
            };
        }

        internal static OntologyModel MapFamily(this OntologyTerm input, ILanguageCache languageCache)
        {
            var result = input.MapSingle(languageCache);
            result.Children = input.Children
                .Select(y => y.Child.MapSingle(languageCache))
                .Where(x => x != null)
                .ToList();
            result.Parents = input.Parents
                .Select(y => y.Parent.MapSingle(languageCache))
                .Where(x => x != null)
                .ToList();
            result.ExactMatches = input.OntologyTermExactMatches?.Select(y => y?.ExactMatch?.Uri).Where(y => y != null)
                .ToList();
            return result;
        }

        internal static IndustrialClassModel ToModel(this IndustrialClass input, ILanguageCache languageCache)
        {
            return new IndustrialClassModel()
            {
                ChildrenIds = input.Children.Select(x => x.Id).ToList(),
                Code = input.Code,
                Descriptions = input.Descriptions
                    .Select(x => new
                        {Language = languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(), x.Description})
                    .ToDictionary(x => x.Language, x => x.Description),
                Id = input.Id,
                Names = input.Names
                    .Select(x => new
                        {Language = languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(), x.Name})
                    .ToDictionary(x => x.Language, x => x.Name),
                ParentId = input.ParentId
            };
        }
        
        internal static DigitalAuthorizationModel ToModel(this DigitalAuthorization input, ILanguageCache languageCache)
        {
            return new DigitalAuthorizationModel()
            {
                Children = input.Children.Select(x => ToModel(x, languageCache)).ToList(),
                Code = input.Code,
                Id = input.Id,
                IsValid = input.IsValid,
                Names = input.Names
                    .Select(x => new
                        {Language = languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(), x.Name})
                    .ToDictionary(x => x.Language, x => x.Name),
                ParentId = input.ParentId
            };
        }

        internal static LifeEventModel ToModel(this LifeEvent input, ILanguageCache languageCache)
        {
            return new LifeEventModel()
            {
                ChildrenIds = input.Children.Select(x => x.Id).ToList(),
                Code = input.Code,
                Id = input.Id,
                Names = input.Names
                    .Select(x => new
                        {Language = languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(), x.Name})
                    .ToDictionary(x => x.Language, x => x.Name),
                ParentId = input.ParentId
            };
        }
        
        internal static ServiceClassModel ToModel(this ServiceClass input, ILanguageCache languageCache)
        {
            return new ServiceClassModel()
            {
                ChildrenIds = input.Children.Select(x => x.Id).ToList(),
                Code = input.Code,
                Descriptions = input.Descriptions
                    .Select(x => new
                        {Language = languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(), x.Description})
                    .ToDictionary(x => x.Language, x => x.Description),
                Id = input.Id,
                Names = input.Names
                    .Select(x => new
                        {Language = languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(), x.Name})
                    .ToDictionary(x => x.Language, x => x.Name),
                ParentId = input.ParentId
            };
        }
        
        internal static TargetGroupModel ToModel(this TargetGroup input, ILanguageCache languageCache)
        {
            return new TargetGroupModel()
            {
                Code = input.Code,
                Id = input.Id,
                Names = input.Names
                    .Select(x => new
                        {Language = languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(), x.Name})
                    .ToDictionary(x => x.Language, x => x.Name),
                ParentId = input.ParentId
            };
        }

        internal static HolidayModel ToModel(this HolidayDate input, ILanguageCache languageCache)
        {
            return new HolidayModel()
            {
                Date = input.Date,
                Id = input.HolidayId,
                Names = input.Holiday.Names
                    .Select(x => new
                        {Language = languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(), x.Name})
                    .ToDictionary(x => x.Language, x => x.Name)
            };
        }

        internal static ServiceNumberModel ToModel(this ServiceNumber input)
        {
            return new ServiceNumberModel()
            {
                Id = input.Id,
                Number = input.Number
            };
        }

        internal static DialCodeModel ToModel(this DialCode input)
        {
            return new DialCodeModel()
            {
                Id = input.Id,
                CountryCode = input.Country.Code,
                Code = input.Code,
            };
        }

        internal static LanguageModel ToModel(this Language input, ILanguageCache languageCache)
        {
            return new LanguageModel()
            {
                Code = input.Code,
                Descriptions = new Dictionary<LanguageEnum, string>(),
                Id = input.Id,
                IsForData = input.IsForData,
                IsForTranslation = input.IsForTranslation,
                Names = input.Names
                    .Select(x => new
                        {Language = languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(), x.Name})
                    .ToDictionary(x => x.Language, x => x.Name),
                Order = input.OrderNumber
            };
        }

        internal static PostalCodeModel ToModel(this PostalCode input, ILanguageCache languageCache)
        {
            return new PostalCodeModel
            {
                Code = input.Code,
                Names = input.PostalCodeNames
                    .Select(x => new
                        {Language = languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(), x.Name})
                    .ToDictionary(x => x.Language, x => x.Name),
            };
        }

        internal static CountryModel ToModel(this Country input, ILanguageCache languageCache)
        {
            return new CountryModel
            {
                Code = input.Code,
                Names = input.CountryNames
                    .Select(x => new
                        {Language = languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(), x.Name})
                    .ToDictionary(x => x.Language, x => x.Name),
            };
        }        
    }
}