/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Channels.V2;
using PTV.Database.Model.Models;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;

namespace PTV.Database.DataAccess.Translators.Common.V2
{
    [RegisterService(typeof(ITranslator<IBaseInformation, VmEntityHeaderBase>), RegisterType.Transient)]
    internal class EntityHeaderTranslator : Translator<IBaseInformation, VmEntityHeaderBase>
    {
        private readonly ILanguageCache languageCache;
        private ILanguageOrderCache languageOrderCache;
        private ITypesCache typesCache;
        public EntityHeaderTranslator(IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            languageOrderCache = cacheManager.LanguageOrderCache;
            typesCache = cacheManager.TypesCache;
        }

        public override VmEntityHeaderBase TranslateEntityToVm(IBaseInformation entity)
        {
            if (entity.PublishingStatusId ==
                typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString()))
            {
                var psdDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
                entity.LanguageAvailabilitiesReference.ForEach(x => x.StatusId = psdDeleted);
            }

            return CreateEntityViewModelDefinition(entity)
                .AddDictionary(input => GetName(input, NameTypeEnum.Name), output => output.Name, name => languageCache.GetByValue(name.LocalizationId))
                .AddNavigation(i => i.Versioning, o => o.Version)
                .AddCollection(i => i.LanguageAvailabilitiesReference.OrderBy(x => languageOrderCache.Get(x.LanguageId)), o => o.LanguagesAvailabilities)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => i.UnificRootId, o => o.UnificRootId)
                .AddSimple(i => i.PublishingStatusId, o => o.PublishingStatus).GetFinal();
        }
        private IEnumerable<IName> GetName(IBaseInformation basicInformation, NameTypeEnum type)
        {
            return basicInformation.Names.Where(x => typesCache.Compare<NameType>(x.TypeId, type.ToString()));
        }

        public override IBaseInformation TranslateVmToEntity(VmEntityHeaderBase vModel)
        {
            throw new NotImplementedException();
        }
    }
}

//using PTV.Database.DataAccess.Caches;
//using PTV.Database.DataAccess.Interfaces.Translators;
//using PTV.Database.Model.Interfaces;
//using PTV.Database.Model.Models;
//using PTV.Domain.Model.Enums;
//using PTV.Domain.Model.Models.V2.Channel;
//using PTV.Framework;
//using PTV.Framework.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace PTV.Database.DataAccess.Translators.Channels.V2
//{
//    [RegisterService(typeof(ITranslator<IBasicInformation<>, VmChannelHeader>), RegisterType.Transient)]
//    internal class ChannelHeaderTranslator : Translator<ServiceChannelVersioned, VmChannelHeader>
//    {
//        private ServiceChannelTranslationDefinitionHelper definitionHelper;
//        private readonly ILanguageCache languageCache;
//        private ILanguageOrderCache languageOrderCache;
//        private ITypesCache typesCache;
//        public ChannelHeaderTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ServiceChannelTranslationDefinitionHelper definitionHelper, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
//        {
//            this.definitionHelper = definitionHelper;
//            typesCache = cacheManager.TypesCache;
//            languageOrderCache = cacheManager.LanguageOrderCache;
//            languageCache = cacheManager.LanguageCache;
//        }

//        public override VmChannelHeader TranslateEntityToVm(ServiceChannelVersioned entity)
//        {
//            var definition = CreateEntityViewModelDefinition(entity)
//                .AddDictionary(input => GetName(input, NameTypeEnum.Name), output => output.Name, name => languageCache.GetByValue(name.LocalizationId))
//                .AddNavigation(i => i.Versioning, o => o.Version)
//                .AddSimple(i => i.Id, o => o.Id);
//            definitionHelper.AddLanguageAvailabilitiesDefinition<ServiceChannelVersioned, VmChannelHeader, ServiceChannelLanguageAvailability>(definition, languageOrderCache);
//            return definition.GetFinal();
//        }

//        public override ServiceChannelVersioned TranslateVmToEntity(VmChannelHeader vModel)
//        {
//            throw new NotImplementedException();
//        }

//        private IEnumerable<IName> GetName(ServiceChannelVersioned serviceChannelVersioned, NameTypeEnum type)
//        {
//            return serviceChannelVersioned.ServiceChannelNames.Where(x => typesCache.Compare<NameType>(x.TypeId, type.ToString()));
//        }
//    }
//}