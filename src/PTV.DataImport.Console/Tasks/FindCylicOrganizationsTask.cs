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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Framework;

namespace PTV.DataImport.Console.Tasks
{
    [RegisterService(typeof(FindCyclicOrganizationsTask), RegisterType.Transient)]
    public class FindCyclicOrganizationsTask
    {
        private readonly IServiceProvider serviceProvider;

        public FindCyclicOrganizationsTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        private bool GoDeep(ref Dictionary<Guid, List< OrganizationVersioned>> data, OrganizationVersioned orgVersioned, OrganizationVersioned origin, ref Dictionary<Guid, Guid> helpResult, bool childGoing = false)
        {
            if (orgVersioned.UnificRootId == orgVersioned.ParentId)
            {
                return true;
            }
            if (!childGoing)
            {
                if (helpResult.Keys.Contains(orgVersioned.UnificRootId))
                {
                    var item = helpResult[orgVersioned.UnificRootId];
                    if (origin != null && item != origin.UnificRootId)
                    {
                        return true;
                    }
                }
                else
                {
                    helpResult.Add(orgVersioned.UnificRootId, origin?.UnificRootId ?? Guid.Empty);
                }
            }
            if (orgVersioned.ParentId != null)
            {
                var topChildrens = data[orgVersioned.ParentId.Value];
                var toCheckChild = topChildrens.First();
                var isBad = GoDeep(ref data, toCheckChild, orgVersioned, ref helpResult);
                if (isBad)
                {
                    return true;
                }
            }
            if (!childGoing)
            {
                var sameChildren = data[orgVersioned.UnificRootId];
                foreach (var nodeChild in sameChildren)
                {
                    var isBad = GoDeep(ref data, nodeChild, orgVersioned, ref helpResult, true);
                    if (isBad)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<Guid> FindCyclicOrganizations()
        {
            System.Console.WriteLine("Searching for cyclic dependencies of organizations...");

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                return scopedCtxMgr.ExecuteReader(unitOfWork =>
                {
                    var orgRep = unitOfWork.CreateRepository<IOrganizationRepository>();
                    var orgVersionedRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();

                    var tempOrgs = orgRep.All().Include(j => j.Versions).ToDictionary(i => i.Id, i => i.Versions.ToList());

                    var wrongs = new Dictionary<Guid, Dictionary<Guid, Guid>>();

                    tempOrgs.ForEach(or =>
                    {
                        or.Value.ForEach(ov =>
                        {
                            var helpResult = new Dictionary<Guid, Guid>();
                            var isBad = GoDeep(ref tempOrgs, ov, null, ref helpResult);
                            if (isBad)
                            {
                                wrongs.Add(ov.Id, helpResult);
                            }
                        });
                    });


                    var wrongIds = wrongs.Keys.ToList();
                    var wrongOrgVersioned = orgVersionedRep.All()
                        .Where(i => wrongIds.Contains(i.Id))
                        .Include(j => j.OrganizationNames)
                        .ThenInclude(j => j.Localization)
                        .OrderBy(i => i.UnificRootId)
                        .ToList();

                    System.Console.WriteLine("Wrong organizations (OrganizationVersioned):");
                    wrongOrgVersioned.ForEach(o =>
                    {
                        System.Console.WriteLine($"Id:{o.Id}, UnificRootId:{o.UnificRootId}, ParentId:{o.ParentId}, Name:{o.OrganizationNames.FirstOrDefault(k => k.Localization.Code.ToLower() == "fi")?.Name ?? string.Empty}, ModifiedBy:{o.ModifiedBy}");
                        var cycles = wrongs[o.Id];
                        cycles.ForEach(c =>
                        {
                            var s = c.Value.IsAssigned() ? c.Value.ToString() : "(root)";
                            System.Console.WriteLine($" * {c.Key} -> {s}");
                        });
                    });
                    return wrongOrgVersioned.Select(i => i.Id).ToList();
                });
            }
        }
    }
}


