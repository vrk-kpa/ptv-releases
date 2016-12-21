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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.ApplicationDbContext;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IAddressService), RegisterType.Transient)]
    public class AddressService : ServiceBase, IAddressService
    {
        private MapServiceProvider mapServiceProvider;
        private readonly IResolveManager resolveManager;
        private readonly IContextManager contextManager;
        public AddressService(IContextManager contextManager, IResolveManager resolveManager, MapServiceProvider mapServiceProvider, ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache) : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache)
        {
            this.resolveManager = resolveManager;
            this.mapServiceProvider = mapServiceProvider;
            this.contextManager = contextManager;
        }

        public VmAddressSimple GetAddress(VmGetCoordinate model)
        {
            VmAddressSimple result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var address = unitOfWork.CreateRepository<IAddressRepository>().All().Where(x => x.Id == model.AddressId);
                address = unitOfWork.ApplyIncludes(address, q => q
                    .Include(x => x.StreetNames)
                    .Include(x => x.PostalCode)
                    .Include(x => x.AddressAdditionalInformations));
                TranslationManagerToVm.SetLanguage(model.Language);
                result = TranslationManagerToVm.Translate<Address, VmAddressSimple>(address.FirstOrDefault());
            });

            return result;
        }

        public void UpdateAddress(IEnumerable<Guid> addressIds)
        {
            resolveManager.RunInThread(rm =>
            {
                IReadOnlyList<AddressInfo> result = null;
                var contextManager = rm.Resolve<IContextManager>();
                var tmToVm = rm.Resolve<ITranslationEntity>();
                var tmToEntity = rm.Resolve<ITranslationViewModel>();

                contextManager.ExecuteReader(unitOfWork =>
                {
                    var addressRepositiory = unitOfWork.CreateRepository<IAddressRepository>();
                    var addresses = addressRepositiory.All().Where(x => addressIds.Contains(x.Id));
                    addresses = unitOfWork.ApplyIncludes(addresses, q =>
                        q.Include(i => i.StreetNames)
                        .Include(i => i.PostalCode).ThenInclude(i => i.Municipality));

                    result = tmToVm.TranslateAll<Address, AddressInfo>(addresses);
                });
                if (!result.IsNullOrEmpty())
                {
                    var coordinates = mapServiceProvider.GetCoordiantes(result).Result;

                    if (!coordinates.IsNullOrEmpty())
                    {
                        contextManager.ExecuteWriter(unitOfWork =>
                        {
                            tmToEntity.TranslateAll<AddressInfo, Address>(coordinates, unitOfWork);
                            unitOfWork.Save();
                        });
                    }
                }
            });
        }
    }
}
