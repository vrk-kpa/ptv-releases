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

using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.UserAccessRightsGroup
{
    [RegisterService(typeof(ITranslator<Model.Models.UserAccessRightsGroup, VmListItem>), RegisterType.Scope)]
    internal class UserAccessRightsGroupTranslator : Translator<Model.Models.UserAccessRightsGroup, VmListItem>
    {
        public UserAccessRightsGroupTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmListItem TranslateEntityToVm(Model.Models.UserAccessRightsGroup entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.Code, o => o.Code)
                .AddPartial<INameReferences, IVmTranslatedItem>(i => i as INameReferences, o => o)
                .AddSimple(i => i.Id, o => o.Id);

            return definition.GetFinal();
        }

        public override Model.Models.UserAccessRightsGroup TranslateVmToEntity(VmListItem vModel)
        {
            throw new System.NotImplementedException();
        }
    }
}
