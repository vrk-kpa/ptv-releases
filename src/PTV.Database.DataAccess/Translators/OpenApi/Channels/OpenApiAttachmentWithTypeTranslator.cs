﻿/**
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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;


namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<Attachment, VmOpenApiAttachmentWithType>), RegisterType.Transient)]
    internal class OpenApiAttachmentWithTypeTranslator : OpenApiAttachmentBaseTranslator<VmOpenApiAttachmentWithType>
    {
        private readonly ITypesCache typesCache;

        public OpenApiAttachmentWithTypeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiAttachmentWithType TranslateEntityToVm(Attachment entity)
        {
            return base.CreateBaseDefinitions(entity)
                .AddNavigation(i => i.Type, o => o.Type)
                .AddNavigation(i => i.TypeId.HasValue ? typesCache.GetByValue<AttachmentType>(i.TypeId.Value) : null, o => o.Type)
                .GetFinal();
        }

        public override Attachment TranslateVmToEntity(VmOpenApiAttachmentWithType vModel)
        {
            return base.CreateVmBaseDefinitions(vModel)
                .AddSimple(i => typesCache.Get<AttachmentType>(i.Type), o => o.TypeId)
                .GetFinal();
        }
    }
}
