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
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Mass;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Mass
{
    [RegisterService(typeof(ITranslator<ILanguageAvailability, VmMassLanguageAvailabilityModel>), RegisterType.Transient)]
    internal class MassLanguageAvailabilityTranslator : Translator<ILanguageAvailability, VmMassLanguageAvailabilityModel>
    {
        public MassLanguageAvailabilityTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmMassLanguageAvailabilityModel TranslateEntityToVm(ILanguageAvailability entity)
        {
            throw new NotImplementedException("Translator ILanguageAvailability -> VmMassLanguageAvailabilityModel is not implemented.");
        }

        public override ILanguageAvailability TranslateVmToEntity(VmMassLanguageAvailabilityModel vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel);

            if (vModel.AllowSaveNull)
            {
                switch (vModel.PublishAction)
                {
                    case PublishActionTypeEnum.ScheduleArchive:
                    case PublishActionTypeEnum.SchedulePublishArchive when !vModel.ValidFrom.HasValue:
                        return definition
                            .AddSimple(i => i.ValidTo, o => o.ArchiveAt)
                            .AddSimple(i=> i.ValidTo.HasValue ? i.Archived : null, o => o.SetForArchived)
                            .AddNavigation(i=> i.ValidTo.HasValue ? i.ArchivedBy : null, o => o.SetForArchivedBy)
                            .AddSimple(i => (DateTime?)null, o => o.LastFailedPublishAt)
                            .GetFinal();                    
                    case PublishActionTypeEnum.SchedulePublish:                    
                        return definition
                            .AddSimple(i => i.ValidFrom, o => o.PublishAt)
                            .AddSimple(i=> i.ValidFrom.HasValue ? i.Reviewed : null, o => o.Reviewed)
                            .AddNavigation(i=> i.ValidFrom.HasValue ? i.ReviewedBy : null, o => o.ReviewedBy)
                            .AddSimple(i => (DateTime?)null, o => o.LastFailedPublishAt)
                            .GetFinal();                    
                    case PublishActionTypeEnum.SchedulePublishArchive:
                        return definition
                            .AddSimple(i => i.ValidTo, o => o.ArchiveAt)
                            .AddSimple(i=> i.ValidTo.HasValue ? i.Archived : null, o => o.SetForArchived)
                            .AddNavigation(i=> i.ValidTo.HasValue ? i.ArchivedBy : null, o => o.SetForArchivedBy)
                            .AddSimple(i => i.ValidFrom, o => o.PublishAt)
                            .AddSimple(i=> i.ValidFrom.HasValue ? i.Reviewed : null, o => o.Reviewed)
                            .AddNavigation(i=> i.ValidFrom.HasValue ? i.ReviewedBy : null, o => o.ReviewedBy)
                            .AddSimple(i => (DateTime?)null, o => o.LastFailedPublishAt)
                            .GetFinal();

                    case null: throw new ArgumentOutOfRangeException();
                    default: throw new ArgumentOutOfRangeException();
                }
            }
            
            if (vModel.ValidFrom.HasValue)
            {
                definition.AddSimple(i => i.ValidFrom, o => o.PublishAt);
                definition.AddSimple(i => (DateTime?) null, o => o.LastFailedPublishAt);
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
            
            if (vModel.Archived.HasValue)
            {
                definition.AddSimple(i => i.Archived, o => o.SetForArchived);
            }

            if (!string.IsNullOrEmpty(vModel.ArchivedBy))
            {
                definition.AddNavigation(i => i.ArchivedBy, o => o.SetForArchivedBy);
            }

            return definition.GetFinal();
        }
    }
}