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
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using PTV.Database.Model.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    public class GroupVersionsTask
    {
        private IServiceProvider serviceProvider;

        public GroupVersionsTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void GroupVersions()
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedContextManager = serviceScope.ServiceProvider.GetService<IContextManager>();

                scopedContextManager.ExecuteWriter(unitOfWork =>
                {
                    var versioningRepository = unitOfWork.CreateRepository<IVersioningRepository>();
                    var versionings = versioningRepository.All()
                        .OrderBy(x => x.VersionMajor)
                        .ToList();
                    var versionsIdsMap = versionings.ToDictionary(x => x.Id);
                    var visitedVersioningIds = new HashSet<Guid>();
                    var counter = 0;
                    var maxCounter = versionings.Count;
                    foreach (var versioning in versionings)
                    {
                        LinkVersions(
                            versioning,
                            versionsIdsMap,
                            visitedVersioningIds
                        );
                        if (++counter % 100 == 0)
                        {
                            Console.Write(
                                "\r{0}%                 ",
                                (((double)counter / maxCounter) * 100).ToString("N2")
                            );
                        }
                    }
                    Console.WriteLine("Saving");
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                });
            }
        }
        private Guid LinkVersions(
            Versioning version,
            IDictionary<Guid, Versioning> versionsIdsMap,
            HashSet<Guid> visitedVersioningIds
        )
        {
            if (version.UnificRootId == null)
            {
                if (
                    visitedVersioningIds.Contains(version.Id) ||
                    version.PreviousVersionId == null ||
                    version.PreviousVersionId == version.Id
                )
                {
                    version.UnificRootId = version.Id;
                    visitedVersioningIds.Add(version.Id);
                    return version.Id;
                }
                visitedVersioningIds.Add(version.Id);
                var previousVersion = versionsIdsMap[version.PreviousVersionId.Value];
                if (previousVersion.UnificRootId != null)
                {
                    version.UnificRootId = previousVersion.UnificRootId;
                    return version.UnificRootId.Value;
                }

                var unificRootId = LinkVersions(
                    previousVersion,
                    versionsIdsMap,
                    visitedVersioningIds
                );
                version.UnificRootId = unificRootId;
                return unificRootId;
            }
            visitedVersioningIds.Add(version.Id);
            return version.UnificRootId.Value;
        }
    }
}
