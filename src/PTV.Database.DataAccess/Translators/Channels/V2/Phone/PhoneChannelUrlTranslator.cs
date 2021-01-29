/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models;
using VmPhoneChannel = PTV.Domain.Model.Models.V2.Channel.VmPhoneChannel;
using VmWebPageChannel = PTV.Domain.Model.Models.V2.Channel.VmWebPageChannel;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.Channels.V2.Phone
{
    [RegisterService(typeof(ITranslator<ServiceChannelWebPage, VmUrl>), RegisterType.Transient)]
    internal class PhoneChannelUrlTranslator : Translator<ServiceChannelWebPage, VmUrl>
    {
        public PhoneChannelUrlTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) 
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmUrl TranslateEntityToVm(ServiceChannelWebPage entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => i.LocalizationId, o => o.LocalizationId)
                .AddSimple(i => i.WebPageId, o => o.WebPageId)
                .AddNavigation(i => i.WebPage, o => o.UrlAddress);
            return definition.GetFinal();
        }

        public override ServiceChannelWebPage TranslateVmToEntity(VmUrl vModel)
        {
            var exists = vModel.Id.IsAssigned();
            var languageId = vModel.LocalizationId.IsAssigned()
                ? vModel.LocalizationId.Value
                : RequestLanguageId;

            return CreateViewModelEntityDefinition<ServiceChannelWebPage>(vModel)
                .UseDataContextCreate(x => !exists)
                .UseDataContextUpdate(x => exists, i => o => i.Id == o.Id)
                .AddSimple(i => languageId, o => o.LocalizationId)
                .AddNavigation(i => i.UrlAddress, o => o.WebPage)
                .GetFinal();
        }

    }
}
