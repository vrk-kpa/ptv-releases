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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<Attachment, VmChannelAttachment>), RegisterType.Transient)]
    internal class AttachmentTranslator : Translator<Attachment, VmChannelAttachment>
    {
        private ITypesCache typesCache;
        public AttachmentTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmChannelAttachment TranslateEntityToVm(Attachment entity)
        {
;
            return CreateEntityViewModelDefinition<VmChannelAttachment>(entity)
                .AddNavigation(i => i.Url, o => o.UrlAddress)
                .AddNavigation(i => i.Name, o => o.Name)
                .AddNavigation(i => i.Description, o => o.Description)
                .AddSimple(i => i.Id, o => o.Id)
                .GetFinal();
        }

        public override Attachment TranslateVmToEntity(VmChannelAttachment vModel)
        {
            bool exists = vModel.Id.IsAssigned();

            var transaltionDefinition = CreateViewModelEntityDefinition<Attachment>(vModel)
               .UseDataContextCreate(input => !exists, output => output.Id, input => Guid.NewGuid())
               .UseDataContextUpdate(input => exists, input => output => input.Id == output.Id)
               .AddNavigation(i => vModel.UrlAddress, o => o.Url)
               .AddNavigation(i => vModel.Name, o => o.Name)
               .AddNavigation(i => vModel.Description, o => o.Description)
               .AddSimple(i => typesCache.Get<AttachmentType>(AttachmentTypeEnum.Attachment.ToString()), o => o.TypeId )
               .AddSimple(i => i.OrderNumber, o => o.OrderNumber)
               .AddRequestLanguage(output => output);

            var entity = transaltionDefinition.GetFinal();
            return entity;
        }

    }
}
