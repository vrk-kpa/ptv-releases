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
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.UserAccessRightsGroup
{
    [RegisterService(typeof(ITranslator<Model.Models.UserAccessRightsGroup, VmJsonUserAccessRightsGroup>), RegisterType.Scope)]
    internal class JsonUserAccessRightsGroupTranslator : Translator<Model.Models.UserAccessRightsGroup, VmJsonUserAccessRightsGroup>
    {
        public JsonUserAccessRightsGroupTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmJsonUserAccessRightsGroup TranslateEntityToVm(Model.Models.UserAccessRightsGroup entity)
        {
            throw new System.NotImplementedException();
        }

        public override Model.Models.UserAccessRightsGroup TranslateVmToEntity(VmJsonUserAccessRightsGroup vModel)
        {
            var definition = CreateViewModelEntityDefinition<Model.Models.UserAccessRightsGroup>(vModel)
                .UseDataContextUpdate(i => true, i => o => i.Code == o.Code, def => def.UseDataContextCreate(x => true, o => o.Id,
                    i => Guid.NewGuid()));

            var entity = definition.GetFinal();
            vModel.Names.ForEach(x => x.OwnerReferenceId = entity.Id);
            return definition
                .AddCollection(input => input.Names, output => output.UserAccessRightsGroupNames)
                .AddNavigation(input => input.Code, output => output.Code)
                .AddNavigation(input => input.UserRole, output => output.UserRole)
                .AddSimple(input => input.AccessRightFlag, output => output.AccessRightFlag)
                .GetFinal();
        }
    }
}
