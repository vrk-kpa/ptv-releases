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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ServiceChannelTranslationDefinitionHelper), RegisterType.Scope)]
    internal class ServiceChannelTranslationDefinitionHelper : EntityDefinitionHelper
    {
        private ILanguageCache languageCache;

        public ServiceChannelTranslationDefinitionHelper(ICacheManager cacheManager)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public ITranslationDefinitions<TIn, ServiceChannel> AddWebPagesDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannel> definition, IWebPages model, Guid? referenceId) where TIn : IWebPages
        {
            model.WebPages?.ForEach(i => i.OwnerReferenceId = referenceId);
            return definition.AddCollection(i => i.WebPages, o => o.WebPages);
        }

        public ITranslationDefinitions<ServiceChannel, TOut> AddWebPagesDefinition<TOut>(ITranslationDefinitions<ServiceChannel, TOut> definition, LanguageCode languageCode) where TOut : IWebPages
        {
            return definition.AddCollection(i => i.WebPages.Where(x => languageCache.Get(languageCode.ToString()) == x.WebPage.LocalizationId), o => o.WebPages);
        }

        public ITranslationDefinitions<TIn, ServiceChannel> AddAttachmentsDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannel> definition, IAttachments model, Guid? referenceId) where TIn : IAttachments
        {
            model.UrlAttachments?.ForEach(i => i.OwnerReferenceId = referenceId);
            return definition.AddCollection(input => input.UrlAttachments, output => output.Attachments);
        }

        public ITranslationDefinitions<ServiceChannel, TOut> AddAttachmentsDefinition<TOut>(ITranslationDefinitions<ServiceChannel, TOut> definition, LanguageCode requestLanguageCode) where TOut : IAttachments
        {
            return definition.AddCollection(input => languageCache.FilterCollection(input.Attachments.Select(i => i.Attachment), requestLanguageCode), output => output.UrlAttachments);
        }

        public ITranslationDefinitions<TIn, ServiceChannel> AddChannelDescriptionsDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannel> definition) where TIn : IVmChannelDescription
        {
            return definition.AddPartial<IVmChannelDescription>(input => input);
        }

        public ITranslationDefinitions<ServiceChannel, TOut> AddChannelDescriptionsDefinition<TOut>(ITranslationDefinitions<ServiceChannel, TOut> definition) where TOut : IVmChannelDescription
        {
            return definition.AddNavigation<ServiceChannel, IVmChannelDescription>(input => input, output => output, true);
        }

        public ITranslationDefinitions<TIn, ServiceChannel> AddEmailDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannel> definition) where TIn : IEmail
        {
            return definition.AddPartial<IEmail>(input => input);
        }

        public ITranslationDefinitions<ServiceChannel, TOut> AddEmailDefinition<TOut>(ITranslationDefinitions<ServiceChannel, TOut> definition) where TOut : IEmail
        {
            return definition.AddNavigation<ServiceChannel, IEmail>(input => input, output => output, true);
        }

        public ITranslationDefinitions<TIn, ServiceChannel> AddPhonesDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannel> definition) where TIn : IPhoneNumberAndFax
        {
            return definition.AddPartial<IPhoneNumberAndFax>(input => input);
        }

        public ITranslationDefinitions<ServiceChannel, TOut> AddPhonesDefinition<TOut>(ITranslationDefinitions<ServiceChannel, TOut> definition) where TOut : IPhoneNumberAndFax
        {
            return definition.AddNavigation<ServiceChannel, IPhoneNumberAndFax>(input => input, output => output, true);
        }

        public ITranslationDefinitions<TIn, ServiceChannel> AddPhoneDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannel> definition) where TIn : IPhoneNumber
        {
            return definition.AddPartial<IPhoneNumber>(input => input);
        }

        public ITranslationDefinitions<ServiceChannel, TOut> AddPhoneDefinition<TOut>(ITranslationDefinitions<ServiceChannel, TOut> definition) where TOut : IPhoneNumber
        {
            return definition.AddNavigation<ServiceChannel, IPhoneNumber>(input => input, output => output, true);
        }

        public ITranslationDefinitions<TIn, ServiceChannel> AddLanguagesDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannel> definition) where TIn : ILanguages
        {
            return definition.AddCollection(i => i.Languages.Select(x => new VmListItem {Id = x}), o => o.Languages);
        }

        public ITranslationDefinitions<ServiceChannel, TOut> AddLanguagesDefinition<TOut>(ITranslationDefinitions<ServiceChannel, TOut> definition) where TOut : ILanguages
        {
            return definition.AddSimpleList(input => input.Languages.Select(x => x.LanguageId), output => output.Languages);
        }

        public ITranslationDefinitions<TIn, ServiceChannelServiceHours> AddOpeningHoursDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannelServiceHours> definition) where TIn : VmHours
        {
            return definition.AddPartial<VmHours>(input => input);
        }

        public ITranslationDefinitions<ServiceChannelServiceHours, TOut> AddOpeningHoursDefinition<TOut>(ITranslationDefinitions<ServiceChannelServiceHours, TOut> definition) where TOut : VmHours
        {
            return definition.AddNavigation<ServiceChannelServiceHours, VmHours>(input => input, output => output, true);
        }
    }

    [RegisterService(typeof(EntityDefinitionHelper), RegisterType.Scope)]
    internal class EntityDefinitionHelper
    {
        public ITranslationDefinitions<TIn, TOut> GetDefinitionWithCreateOrUpdate<TIn, TOut>(ITranslationDefinitions<TIn, TOut> definition) where TIn : VmEntityBase where TOut : IEntityIdentifier
        {
            return definition
                .UseDataContextCreate(input => !input.Id.IsAssigned(), output => output.Id, input => Guid.NewGuid())
                .UseDataContextUpdate(input => input.Id.IsAssigned(), input => output => input.Id == output.Id);

        }
    }
}