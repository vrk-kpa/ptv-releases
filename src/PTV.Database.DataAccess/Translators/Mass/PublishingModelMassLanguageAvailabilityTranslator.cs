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
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Mass;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Mass
{
    [RegisterService(typeof(ITranslator<VmPublishingModel, VmMassLanguageAvailabilityModelList>), RegisterType.Transient)]
    internal class PublishingModelMassLanguageAvailabilityTranslator : Translator<VmPublishingModel, VmMassLanguageAvailabilityModelList>
    {
        public PublishingModelMassLanguageAvailabilityTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmMassLanguageAvailabilityModelList TranslateEntityToVm(VmPublishingModel entity)
        {
            var model = new VmMassLanguageAvailabilityModelList();
            
            model.LanguageAvailabilities.AddRange(entity.LanguagesAvailabilities.Select(x =>
                new VmMassLanguageAvailabilityModel
                {
                    Id = entity.Id,
                    LanguageId = x.LanguageId,
                    ValidFrom = x.ValidFrom?.FromEpochTime(),
                    ValidTo = x.ValidTo?.FromEpochTime(),
                    Reviewed = DateTime.UtcNow,
                    ReviewedBy = entity.UserName,
                }).ToList()
            );

            return model;
        }

        public override VmPublishingModel TranslateVmToEntity(VmMassLanguageAvailabilityModelList vModel)
        {
             throw new NotImplementedException();
        }
    }
}
