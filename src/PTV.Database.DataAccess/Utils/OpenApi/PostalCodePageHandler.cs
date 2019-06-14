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

using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal class PostalCodePageHandler : GuidPageWithNameHandlerBase<VmOpenApiCodeListPage, VmOpenApiCodeListItem, PostalCode>
    {
        private ILanguageCache languageCache;
        private Dictionary<Guid, List<VmOpenApiLanguageItem>> names;

        public PostalCodePageHandler(
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
                Entities = query.ToList();
                // Get names
                names = unitOfWork.CreateRepository<IRepository<PostalCodeName>>().All().Where(x => EntityIds.Contains(x.PostalCodeId)).OrderBy(i => i.Localization.OrderNumber)
                    .Select(i => new { id = i.PostalCodeId, i.Name, i.LocalizationId }).ToList().GroupBy(i => i.id)
                    .ToDictionary(i => i.Key, i => i.ToList().Select(x => new VmOpenApiLanguageItem
                    {
                        Value = x.Name,
                        Language = languageCache.GetByValue(x.LocalizationId),
                    }).ToList());
            }

            return TotalCount;
        }

        protected override VmOpenApiCodeListItem GetItemData(PostalCode entity)
        {
            return new VmOpenApiCodeListItem
            {
                Code = entity.Code,
                Names = names.TryGetOrDefault(entity.Id, null)
            };
        }
    }
}
