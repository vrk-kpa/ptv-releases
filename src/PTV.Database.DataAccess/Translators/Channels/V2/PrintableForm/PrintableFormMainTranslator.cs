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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.V2.Channel.PrintableForm;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.Channels.V2.PrintableForm
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmPrintableForm>), RegisterType.Transient)]
    internal class PrintableFormMainTranslator : Translator<ServiceChannelVersioned, VmPrintableForm>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private readonly ServiceChannelTranslationDefinitionHelper definitionHelper;

        public PrintableFormMainTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ServiceChannelTranslationDefinitionHelper definitionHelper
        ) : base(resolveManager, translationPrimitives)
        {
            typesCache = definitionHelper.CacheManager.TypesCache;
            languageCache = definitionHelper.CacheManager.LanguageCache;
            this.definitionHelper = definitionHelper;
        }

        public override VmPrintableForm TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddCollection(i => i.Addresses, i => i.DeliveryAddresses)
                .AddDictionary(
                    input => input.PrintableFormChannels.FirstOrDefault()?.FormIdentifiers.Select(x => x),
                    output => output.FormIdentifier,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .AddDictionaryList(
                    input => input.PrintableFormChannels.FirstOrDefault()?.ChannelUrls,
                    output => output.FormFiles,
                    k => languageCache.GetByValue(k.LocalizationId)
                );
            definitionHelper
                .AddLanguagesDefinition(definition)
                .AddAttachmentsDefinition(definition, entity)
                .AddChannelBaseDefinition(definition);

            definitionHelper.AddOrderedDictionaryList(
                definition,
                input => input.PrintableFormChannels.FirstOrDefault()?.ChannelUrls,
                o => o.FormFiles,
                k => languageCache.GetByValue(k.LocalizationId)
            );
            return definition.GetFinal();
        }
        
        public override ServiceChannelVersioned TranslateVmToEntity(VmPrintableForm vm)
        {
            var definition = CreateViewModelEntityDefinition(vm)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextLocalizedUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.Id)
                .UseVersioning<ServiceChannelVersioned, ServiceChannel>(o => o)
                .AddLanguageAvailability(i => i, o => o)
                .AddSimple(i => typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.PrintableForm.ToString()), o => o.TypeId)
                .AddNavigationOneMany(i => i, o => o.PrintableFormChannels);
            definitionHelper
                .AddLanguagesDefinition(definition)
                .AddAttachmentsDefinition(definition, vm, vm.Id)
                .AddChannelBaseDefinition(definition);

            definition.Propagation((m, s) =>
            {
                var order = 1;
                vm.DeliveryAddresses?.Where(item => item != null).ForEach(item =>
                {
                    item.AddressCharacter = AddressCharacterEnum.Delivery;
                    item.OrderNumber = order++;
                    item.OwnerReferenceId = s.Id;
                });
            });
            
            definition.AddCollectionWithRemove(i => i.DeliveryAddresses, o => o.Addresses, x => true);
            
            return definition.GetFinal();
        }
    }
}