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
* THE SOFTWARE.C:\Projects\PTV_TEST\src\PTV.Database.DataAccess\Services\Security\
*/
using System;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Services.Validation
{
    [RegisterService(typeof(IValidationChecker<AccessibilityClassification>), RegisterType.Transient)]
    internal class AccessibilityClassificationChecker : BaseValidationChecker<AccessibilityClassification>
    {
        private readonly ITypesCache typesCache;

        public AccessibilityClassificationChecker(ICacheManager cacheManager, IResolveManager resolveManager) : base(cacheManager, resolveManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        protected AccessibilityClassification FetchEntity(Guid id, IUnitOfWork unitOfWork)
        {
            return GetEntity<AccessibilityClassification>(id, unitOfWork, x => x);
        }

        protected override void ValidateEntityInternal(Guid? language)
        {
            if (!language.IsAssigned())
            {
                throw new ArgumentNullException(nameof(language), "language must be defined.");
            }

            var accessibilityClassificationLevelType = typesCache.GetByValue<AccessibilityClassificationLevelType>(entity.AccessibilityClassificationLevelTypeId);
            var accessibilityClassificationLevelTypeEnum = Enum.Parse(typeof(AccessibilityClassificationLevelTypeEnum), accessibilityClassificationLevelType);

            switch (accessibilityClassificationLevelTypeEnum)
            {
                case AccessibilityClassificationLevelTypeEnum.FullyCompliant:
                case AccessibilityClassificationLevelTypeEnum.PartiallyCompliant:
                    NotEmptyGuid("wcagLevelType", x => x.WcagLevelTypeId);
                    NotBeTrue("name", x => x.LocalizationId == language && string.IsNullOrEmpty(x.Name));
                    NotBeTrue("urlAddress", x => x.LocalizationId == language && string.IsNullOrEmpty(x.Url));
                    break;
                case AccessibilityClassificationLevelTypeEnum.NonCompliant:
                    NotBeTrue("name", x => x.LocalizationId == language && string.IsNullOrEmpty(x.Name));
                    NotBeTrue("urlAddress", x => x.LocalizationId == language && string.IsNullOrEmpty(x.Url));
                    break;
                default:
                    break;
            }
        }
    }
}
