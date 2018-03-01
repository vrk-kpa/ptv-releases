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
* THE SOFTWARE.C:\Projects\PTV_TEST\src\PTV.Database.DataAccess\Services\Security\
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Services.Validation
{
    [RegisterService(typeof(IBaseValidationChecker<ServiceLocationChannel>), RegisterType.Transient)]
    internal class ServiceLocationChannelValidationChecker : BaseValidationChecker<ServiceLocationChannel>
    {
        private readonly ITypesCache typesCache;

        public ServiceLocationChannelValidationChecker(ICacheManager cacheManager, IResolveManager resolveManager) : base(cacheManager, resolveManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override ServiceLocationChannel FetchEntity(Guid id, IUnitOfWork unitOfWork)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<Guid, List<ValidationMessage>> ValidateEntity(Guid? language)
        {
            if (!language.IsAssigned())
            {
                throw new ArgumentNullException(nameof(language), "language must be defined.");
            }

            //Visiting Addresses
            if (!NotEmptyList("visitingAddresses", x => entity.Addresses.Where(y => y.CharacterId == typesCache.Get<AddressCharacter>(AddressCharacterEnum.Visiting.ToString()))))
            {
                foreach (var visitingAddress in entity.Addresses.Where(x => x.CharacterId == typesCache.Get<AddressCharacter>(AddressCharacterEnum.Visiting.ToString()))
                    .Select(x => x.Address))
                {
                    CheckEntityWithMergeResult(visitingAddress);
                }
            }

            //Postal Address
            foreach (var postalAddress in entity.Addresses.Where(x => x.CharacterId == typesCache.Get<AddressCharacter>(AddressCharacterEnum.Postal.ToString())).Select(x => x.Address))
            {
                CheckEntityWithMergeResult(postalAddress);
            }

            return validationMessagesDictionary;
        }
    }
}
