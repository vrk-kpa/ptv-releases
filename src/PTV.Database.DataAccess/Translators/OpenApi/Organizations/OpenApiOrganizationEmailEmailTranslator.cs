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
**/

using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    [RegisterService(typeof(ITranslator<Email, VmOpenApiOrganizationEmail>), RegisterType.Transient)]
    internal class OpenApiOrganizationEmailEmailTranslator : Translator<Email, VmOpenApiOrganizationEmail>
    {

        public OpenApiOrganizationEmailEmailTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {}

        public override VmOpenApiOrganizationEmail TranslateEntityToVm(Email entity)
        {
            var definitions = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.Value, o => o.Email);

            if (!string.IsNullOrEmpty(entity.Description))
            {
                definitions.AddCollection(i => new List<Email> { i }, o => o.Descriptions);
            }

            return definitions.GetFinal();
        }

        public override Email TranslateVmToEntity(VmOpenApiOrganizationEmail vModel)
        {
            if (vModel.Id.IsAssigned())
            {
                vModel.Descriptions.ForEach(d => d.OwnerReferenceId = vModel.Id);
            }
            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.Id, e => e.UseDataContextCreate(x => true))
                .AddNavigation(i => i.Email, o => o.Value)
                .AddRequestLanguage(i => i);

            return definition.GetFinal();
        }
    }
}
