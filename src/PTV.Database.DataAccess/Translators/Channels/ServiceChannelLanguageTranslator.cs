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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelLanguage, VmListItem>), RegisterType.Transient)]
    internal class ServiceChannelLanguageTranslator : Translator<ServiceChannelLanguage, VmListItem>
    {
        public ServiceChannelLanguageTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmListItem TranslateEntityToVm(ServiceChannelLanguage entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.LanguageId, o => o.Id)
                .AddLocalizable(i => i.Language.Names, o => o.Name)
                .AddNavigation(i => i.Language.Code, o => o.Code)
                .AddPartial(i => i as IOrderable, o => o as IVmOrderable)
                .GetFinal();
        }

        public override ServiceChannelLanguage TranslateVmToEntity(VmListItem vModel)
        {
            return CreateViewModelEntityDefinition<ServiceChannelLanguage>(vModel)
                .UseDataContextUpdate(
                  input => input.OwnerReferenceId.IsAssigned(),
                  i => o => i.OwnerReferenceId == o.ServiceChannelVersionedId && i.Id == o.LanguageId,
                  def => def.UseDataContextCreate(i => true)
                )
                .AddSimple(input => input.Id, output => output.LanguageId)
                .AddPartial(i => i as IVmOrderable, o => o as IOrderable)
                .GetFinal();
        }
    }
}
