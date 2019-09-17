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
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Database.DataAccess.Translators.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationEmail, string>), RegisterType.Transient)]
    internal class OrganizationEmailTranslator : Translator<OrganizationEmail, string>
    {
        public OrganizationEmailTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override string TranslateEntityToVm(OrganizationEmail entity)
        {
            throw new NotImplementedException();
        }
        public override OrganizationEmail TranslateVmToEntity(string vModel)
        {
            return CreateViewModelEntityDefinition<OrganizationEmail>(vModel)
                .AddNavigation(i => new VmEmailData { Email = vModel}, o => o.Email)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<OrganizationEmail, VmEmailData>), RegisterType.Transient)]
    internal class OrganizationEmailDataTranslator : Translator<OrganizationEmail, VmEmailData>
    {
        public OrganizationEmailDataTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmEmailData TranslateEntityToVm(OrganizationEmail entity)
        {
            throw new NotSupportedException();
            //return CreateEntityViewModelDefinition<VmEmailData>(entity)
            //   .AddSimple(input => input.Organization.Id, output => output.OwnerReferenceId)
            //   .AddSimple(input => input.Id, output => output.Id)
            //   .AddNavigation(input => input.Email, output => output.Email)
            //   .AddNavigation(input => input.OrganizationEmailDescriptions.Select(x => x.Description).FirstOrDefault(), output => output.AdditionalInformation)
            //   .GetFinal();
        }
        public override OrganizationEmail TranslateVmToEntity(VmEmailData vModel)
        {
            if (vModel == null)
            {
                return null;
            }
            bool exists = vModel.Id.IsAssigned();

            var translation = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextLocalizedUpdate(input => exists, input => output => (input.Id == output.EmailId) &&
                                                                          (!input.OwnerReferenceId.IsAssigned() || output.OrganizationVersionedId == vModel.OwnerReferenceId));

            return translation
                .AddNavigation(input => input, output => output.Email)
                .AddPartial(i => i as IVmOrderable, o => o as IOrderable)
                .GetFinal();
        }
    }
}
