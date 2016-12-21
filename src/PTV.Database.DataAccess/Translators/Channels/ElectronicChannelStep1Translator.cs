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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ElectronicChannel, VmElectronicChannelStep1>), RegisterType.Transient)]
    internal class ElectronicChannelStep1Translator : Translator<ElectronicChannel, VmElectronicChannelStep1>
    {
        public ElectronicChannelStep1Translator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmElectronicChannelStep1 TranslateEntityToVm(ElectronicChannel entity)
        {
            throw new NotImplementedException();
        }

        public override ElectronicChannel TranslateVmToEntity(VmElectronicChannelStep1 vModel)
        {
            if (vModel.WebPage != null)
            {
                vModel.WebPage.OwnerReferenceId = vModel.ElectronicChannelId;
            }
//            vModel.UrlAttachments?.ForEach(i => i.OwnerReferenceId = vModel.ElectronicChannelId);
            return CreateViewModelEntityDefinition<ElectronicChannel>(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.ServiceChannelId)
                .AddSimple(i => i.IsOnLineSign, o => o.RequiresSignature)
                .AddSimple(i => i.NumberOfSigns, o => o.SignatureQuantity)
                .AddSimple(i => i.IsOnLineAuthentication ?? false, o => o.RequiresAuthentication)
                .AddLocalizable(i => i.WebPage, o => o.LocalizedUrls).GetFinal();
        }
    }
}
