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

using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models;
using PTV.ExternalSources;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IMigrationService), RegisterType.Transient)]
    internal class MigrationService : IMigrationService
    {
        private IContextManager contextManager;

        public MigrationService(IContextManager contextManager)
        {
            this.contextManager = contextManager;
        }
        
        public void RemoveOldSahaGuids()
        {
            var resourceManager = new ResourceManager();
            var sahaGuids = resourceManager.GetDesrializedJsonResource<List<VmTempSahaGuids>>(JsonResources.TempSahaMapping);

            contextManager.ExecuteWriter(unitOfWork =>
            {
                var sahaInfoRep = unitOfWork.CreateRepository<ISahaOrganizationInformationRepository>();
                var mappingToDelete =
                    sahaInfoRep.All().Where(x => sahaGuids.Select(y => y.OldSahaGuid).Contains(x.SahaId));
                sahaInfoRep.Remove(mappingToDelete);
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }
    }
}