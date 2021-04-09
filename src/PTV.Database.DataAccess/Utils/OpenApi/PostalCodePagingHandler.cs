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

using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal class PostalCodePagingHandler : PagingHandlerBase<VmOpenApiCodeListPage, VmOpenApiCodeListItem>
    {
        private List<Guid> entityIds;
        private List<PostalCode> postalCodes;
        private ILanguageCache languageCache;
        private Dictionary<Guid, List<VmOpenApiLanguageItem>> names;

        List<Guid> postalCodeIds { get
            {
                if (entityIds != null)
                {
                    return entityIds;
                }
                if (postalCodes?.Count > 0)
                {
                    entityIds = postalCodes.Select(i => i.Id).ToList();
                }
                else
                {
                    entityIds = new List<Guid>();
                }
                return entityIds;
            } }

        public PostalCodePagingHandler(
            ILanguageCache languageCache,
            int pageNumber,
            int pageSize
            ) : base(pageNumber, pageSize)
        {
            this.languageCache = languageCache;
        }

        public override int Search(IUnitOfWork unitOfWork)
        {
            if (PageNumber <= 0) return 0;

            var repository = unitOfWork.CreateRepository<IRepository<PostalCode>>();
            var query = repository.All();

            // Get entities total count.
            SetPageCount(query.Count());
            if (ValidPageNumber)
            {
                query = query.OrderBy(o => o.Code).Skip(GetSkipSize()).Take(GetTakeSize());
                postalCodes = query.ToList();
                // Get names
                names = unitOfWork.CreateRepository<IRepository<PostalCodeName>>().All().Where(x => postalCodeIds.Contains(x.PostalCodeId)).OrderBy(i => i.Localization.OrderNumber)
                    .Select(i => new { id = i.PostalCodeId, i.Name, i.LocalizationId }).ToList().GroupBy(i => i.id)
                    .ToDictionary(i => i.Key, i => i.ToList().Select(x => new VmOpenApiLanguageItem
                    {
                        Value = x.Name,
                        Language = languageCache.GetByValue(x.LocalizationId),
                    }).ToList());
            }

            return TotalCount;
        }

        public override IVmOpenApiModelWithPagingBase<VmOpenApiCodeListItem> GetModel()
        {
            if (postalCodes?.Count > 0)
            {
                ViewModel.ItemList = postalCodes.Select(entity => new VmOpenApiCodeListItem
                {
                    Code = entity.Code,
                    Names = names.TryGetOrDefault(entity.Id, null)
                }).ToList();
            }

            return ViewModel;
        }
    }
}
