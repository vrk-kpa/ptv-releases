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
using PTV.Database.DataAccess.Services.V2;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Mass
{
    [RegisterService(typeof(ITranslator<OrganizationLanguageAvailability, VmMassLanguageAvailabilityModel>), RegisterType.Transient)]
    internal class OrganizationLanguageAvailabilityTranslator : Translator<OrganizationLanguageAvailability, VmMassLanguageAvailabilityModel>
    {
        public OrganizationLanguageAvailabilityTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmMassLanguageAvailabilityModel TranslateEntityToVm(OrganizationLanguageAvailability entity)
        {
            throw new NotImplementedException();
        }

        public override OrganizationLanguageAvailability TranslateVmToEntity(VmMassLanguageAvailabilityModel vModel)
        {
            var definition =  CreateViewModelEntityDefinition<OrganizationLanguageAvailability>(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(x => true, i => o => i.Id == o.OrganizationVersionedId && i.LanguageId == o.LanguageId);

            if (vModel.AllowSaveNull)
            {
                definition
                    .AddSimple(i=> i.ValidFrom, o => o.PublishAt)
                    .AddSimple(i=> i.ValidTo, o => o.ArchiveAt)
                    .AddSimple(i=> i.Reviewed, o => o.Reviewed)
                    .AddNavigation(i=> i.ReviewedBy, o => o.ReviewedBy)
                    .GetFinal();
                
                return definition.GetFinal();
            }
            
            if (vModel.ValidFrom.HasValue)
            {
                definition.AddSimple(i => i.ValidFrom, o => o.PublishAt);
            }
            
            if (vModel.ValidTo.HasValue)
            {
                definition.AddSimple(i => i.ValidTo, o => o.ArchiveAt);
            }
            
            if (vModel.Reviewed.HasValue)
            {
                definition.AddSimple(i => i.Reviewed, o => o.Reviewed);
            }

            if (!string.IsNullOrEmpty(vModel.ReviewedBy))
            {
                definition.AddNavigation(i => i.ReviewedBy, o => o.ReviewedBy);
            }

            return definition.GetFinal();
        }
    }
}
