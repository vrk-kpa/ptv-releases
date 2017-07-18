using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Framework.TextManager;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    [RegisterService(typeof(FindCyclicOrganizationsTask), RegisterType.Transient)]
    public class FindCyclicOrganizationsTask
    {
        private IServiceProvider _serviceProvider;

        public FindCyclicOrganizationsTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
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
                bool isBad = GoDeep(ref data, toCheckChild, orgVersioned, ref helpResult);
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
                    bool isBad = GoDeep(ref data, nodeChild, orgVersioned, ref helpResult, true);
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

            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
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
                            bool isBad = GoDeep(ref tempOrgs, ov, null, ref helpResult);
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
                        Console.WriteLine($"Id:{o.Id}, UnificRootId:{o.UnificRootId}, ParentId:{o.ParentId}, Name:{o.OrganizationNames.FirstOrDefault(k => k.Localization.Code.ToLower() == "fi")?.Name ?? string.Empty}, ModifiedBy:{o.ModifiedBy}");
                        var cycles = wrongs[o.Id];
                        cycles.ForEach(c =>
                        {
                            string s = c.Value.IsAssigned() ? c.Value.ToString() : "(root)";
                            Console.WriteLine($" * {c.Key} -> {s}");
                        });
                    });
                    return wrongOrgVersioned.Select(i => i.Id).ToList();
                });
            }
        }
    }
}

    