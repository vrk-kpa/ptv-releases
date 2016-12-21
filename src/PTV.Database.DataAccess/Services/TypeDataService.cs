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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using Microsoft.EntityFrameworkCore;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Domain.Model.Models.Localization;
using PTV.Database.DataAccess.Caches;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ITypeDataService), RegisterType.Transient)]
    public class TypeDataService : ServiceBase, ITypeDataService
    {
        private readonly IContextManager contextManager;

        public TypeDataService(IContextManager contextManager, ITranslationEntity translationManagerToVm, ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache) : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache)
        {
            this.contextManager = contextManager;
        }

        public IVmTranslatedData GetTranslatedData()
        {
            var result = new VmTranslatedData();
            contextManager.ExecuteReader(unitOfWork =>
            {
                List<VmJsonTypeName> names = GetNames<ProvisionTypeName>(unitOfWork).ToList();
                names.AddRange(GetNames<ExceptionHoursStatusTypeName>(unitOfWork));
                names.AddRange(GetNames<PhoneNumberTypeName>(unitOfWork));
                names.AddRange(GetNames<ServiceCoverageTypeName>(unitOfWork));
                names.AddRange(GetNames<ServiceChannelTypeName>(unitOfWork));
                names.AddRange(GetNames<WebPageTypeName>(unitOfWork));
                names.AddRange(GetNames<PrintableFormChannelUrlTypeName>(unitOfWork));
                names.AddRange(GetNames<OrganizationTypeName>(unitOfWork));
                names.AddRange(GetNames<ServiceChargeTypeName>(unitOfWork));
                names.AddRange(GetNames<ServiceTypeName>(unitOfWork));
                //                var provisionTypeNameRep = unitOfWork.CreateRepository<IProvisionTypeNameRepository>();
                //                var data = unitOfWork.ApplyIncludes(provisionTypeNameRep.All(), q => q.Include(x => x.Localization)).ToList();
                //                result.Translations = data.GroupBy(x => x.Localization.Code)
                //                    .ToDictionary(
                //                        type => type.Key,
                //                        type => type.ToDictionary(x => x.ProvisionTypeId.ToString(), x => x.Name) as IDictionary<string, string>);

                result.Translations = names.GroupBy(x => x.Language)
                    .Select<IGrouping<string, VmJsonTypeName>, IVmTranslatation>(type => new VmTranslation {
                        LanguageCode = type.Key,
                        Texts = type.Distinct(new TypeNameComparer()).ToDictionary(x => x.TypeId.ToString(), x => x.Name) as IDictionary<string, string>
                    }).ToList();


            });
            return result;
        }

        private IReadOnlyList<VmJsonTypeName> GetNames<T>(IUnitOfWork unitOfWork) where T : NameBase
        {
            IRepository<T> rep = unitOfWork.CreateRepository<IRepository<T>>();
            var data = unitOfWork.ApplyIncludes(rep.All(), q => q.Include(x => x.Localization));
            return TranslationManagerToVm.TranslateAll<T, VmJsonTypeName>(data);
        }

    }

    internal class TypeNameComparer : IEqualityComparer<VmJsonTypeName>
    {
        public bool Equals(VmJsonTypeName x, VmJsonTypeName y)
        {
            return x.TypeId == y.TypeId;
        }

        public int GetHashCode(VmJsonTypeName obj)
        {
            return obj.TypeId.GetHashCode();
        }
    }
}
