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
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels.V2.Common
{
    [RegisterService(typeof(ITranslator<ServiceChannelSocialHealthCenter, VmChannelHeader>), RegisterType.Transient)]
    internal class SocialHealthCenterTranslator : Translator<ServiceChannelSocialHealthCenter, VmChannelHeader>
    {
        public SocialHealthCenterTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmChannelHeader TranslateEntityToVm(ServiceChannelSocialHealthCenter entity)
        {
            throw new NotImplementedException("Translator ServiceChannelSocialHealthCenter -> VmChannelHeader is not implemented.");
        }

        public override ServiceChannelSocialHealthCenter TranslateVmToEntity(VmChannelHeader vModel)
        {
            if (vModel == null) return null;
            if (vModel.Oid.IsNullOrEmpty()) return null;

            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(i => true, i => o => i.UnificRootId == o.ServiceChannelId, def => def.UseDataContextCreate(i => true))
                .AddSimple(i => i.UnificRootId, o => o.ServiceChannelId)
                .AddNavigation(i => i.Oid, o => o.Oid)
                .GetFinal();
        }
    }
}