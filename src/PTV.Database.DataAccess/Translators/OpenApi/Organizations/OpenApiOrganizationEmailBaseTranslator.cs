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
using PTV.Framework.Interfaces;
using PTV.Framework;
using PTV.Domain.Model.Models.Interfaces.OpenApi;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    internal abstract class OpenApiOrganizationEmailBaseTranslator<TModel> : Translator<OrganizationEmail, TModel> where TModel : class, IVmOpenApiOrganizationEmail
    {

        protected OpenApiOrganizationEmailBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override TModel TranslateEntityToVm(OrganizationEmail entity)
        {
            return CreateBaseEntityVmDefinitions(entity).GetFinal();
        }

        public override OrganizationEmail TranslateVmToEntity(TModel vModel)
        {
            return CreateBaseVmEntityDefinitions(vModel).GetFinal();
        }

        protected ITranslationDefinitions<OrganizationEmail, TModel> CreateBaseEntityVmDefinitions(OrganizationEmail entity)
        {
            throw new NotImplementedException("Translator OrganizationEmail -> IVmOpenApiOrganizationEmail is not implemented");
        }

        protected ITranslationDefinitions<TModel, OrganizationEmail> CreateBaseVmEntityDefinitions(TModel vModel)
        {
            if (vModel == null)
            {
                return null;
            }

            var exists = vModel.OwnerReferenceId.IsAssigned();

            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextUpdate(input => exists, input => output => input.OwnerReferenceId == output.OrganizationVersionedId &&
                    input.Email == output.Email.Value, def => def.UseDataContextCreate(x => true));

            var entity = definition.GetFinal();
            if(entity.Created != DateTime.MinValue)
            {
                vModel.Id = entity.EmailId;
            }

            definition.AddNavigation(input => input, output => output.Email);
            return definition;
        }
    }
}
