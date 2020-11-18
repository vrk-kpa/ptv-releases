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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels.V2.Common
{
    [RegisterService(typeof(ITranslator<ServiceChannelSocialHealthCenter, IVmStringItem>), RegisterType.Transient)]
    internal class SocialHealthCenterTranslator : Translator<ServiceChannelSocialHealthCenter, IVmStringItem>
    {
        public SocialHealthCenterTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override IVmStringItem TranslateEntityToVm(ServiceChannelSocialHealthCenter entity)
        {
            throw new NotImplementedException("Translator ServiceChannelSocialHealthCenter -> IVmStringItem is not implemented.");
        }

        public override ServiceChannelSocialHealthCenter TranslateVmToEntity(IVmStringItem vModel)
        {
            if (vModel == null) return null;
            if (vModel.Value.IsNullOrEmpty()) return null;

            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(i => true, i => o => i.OwnerReferenceId == o.ServiceChannelId, def => def.UseDataContextCreate(i => true))
                .AddSimple(i => i.OwnerReferenceId.Value, o => o.ServiceChannelId)
                .AddNavigation(i => i.Value, o => o.Oid)
                .GetFinal();
        }
    }
}
