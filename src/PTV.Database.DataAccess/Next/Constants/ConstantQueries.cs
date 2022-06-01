using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next.Constants
{
    [RegisterService(typeof(IConstantQueries), RegisterType.Transient)]
    internal class ConstantQueries : IConstantQueries
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;
        public ConstantQueries(ILanguageCache languageCache, ITypesCache typesCache)
        {
            this.languageCache = languageCache;
            this.typesCache = typesCache;
        }
        
        public Dictionary<string, Dictionary<string, Dictionary<LanguageEnum, string>>> GetTexts()
        {
            var result = new Dictionary<string, Dictionary<string, Dictionary<LanguageEnum, string>>>();
            
            AddType<AccessibilityClassificationLevelType>(result);
            AddType<AreaInformationType>(result);
            AddType<AreaType>(result);
            AddType<ExtraSubType>(result, "astiType");
            AddType<ExtraType>(result);
            AddType<ServiceFundingType>(result, "fundingType");
            AddType<GeneralDescriptionType>(result);
            AddType<ServiceChannelType>(result, "channelType");
            AddType<ServiceChargeType>(result, "chargeType");
            AddType<JobExecutionType>(result);
            AddType<JobStatusType>(result);
            AddType<OrganizationType>(result);
            AddType<PhoneNumberType>(result);
            AddType<PrintableFormChannelUrlType>(result);
            AddType<ProvisionType>(result);
            AddType<PublishingStatusType>(result, "publishingStatus");
            AddType<ServiceChannelConnectionType>(result);
            AddType<ServiceType>(result);
            AddType<TranslationStateType>(result);
            AddType<WcagLevelType>(result);

            return result;
        }

        private void AddType<T>(Dictionary<string, Dictionary<string, Dictionary<LanguageEnum, string>>> result, string typeName) where T : IType
        {
            var cacheData = typesCache.GetCacheData<T>();
            var codes = cacheData.Select(x => (x.Code, Names: AddNames(x.Names))).ToDictionary(x => x.Code, x => x.Names);
            result.Add(typeName, codes);
        }

        private void AddType<T>(Dictionary<string, Dictionary<string, Dictionary<LanguageEnum, string>>> result) where T : IType
        {
            var typeName = typeof(T).Name;
            var name = char.ToLowerInvariant(typeName[0]) + typeName.Substring(1);
            AddType<T>(result, name);
        }

        private Dictionary<LanguageEnum, string> AddNames(List<VmTypeName> names)
        {
            return names.ToDictionary(x => languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(),
                x => x.Name);
        }
    }
}