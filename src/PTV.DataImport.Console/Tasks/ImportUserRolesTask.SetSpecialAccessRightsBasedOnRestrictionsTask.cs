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
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    public partial class ImportUserRolesTask
    {
        public class SetSpecialAccessRightsBasedOnRestrictionsTask
        {
            private IServiceProvider _serviceProvider;
            private ILogger _logger;

            public SetSpecialAccessRightsBasedOnRestrictionsTask(IServiceProvider serviceProvider)
            {
                if (serviceProvider == null)
                {
                    throw new ArgumentNullException(nameof(serviceProvider));
                }

                _serviceProvider = serviceProvider;
                _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<CreatePostalCodesJsonTask>();
                _logger.LogDebug("SetSpecialAccessRightsBasedOnRestrictionsTask .ctor");
            }

            public void SetAccessRights()
            {
                var contextManager = _serviceProvider.GetService<IContextManager>();
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var uiAccessRep = unitOfWork.CreateRepository<IAccessRightsOperationsUIRepository>();
                    var typeFilters = unitOfWork.CreateRepository<IRestrictionFilterRepository>();
                    var gdTypeRep = unitOfWork.CreateRepository<IGeneralDescriptionTypeRepository>();
                    var gdFilters = typeFilters.All().Where(i => gdTypeRep.All().Select(j => j.Id).Contains(i.RestrictedType.Value) && i.FilterType == ERestrictionFilterType.Allowed).ToList();
                    gdFilters.ForEach(i => i.BlockOtherTypes = true);
                    var orgsWithAllowedGds = gdFilters.SelectMany(i => i.OrganizationFilters.Select(j => j.OrganizationId)).Distinct().ToList();
                    var existing = uiAccessRep.All().Where(i => i != null && orgsWithAllowedGds.Contains(i.OrganizationId.Value)).Select(i => i.OrganizationId.Value).ToList();
                    orgsWithAllowedGds.Except(existing).ForEach(toAdd =>
                    {
                        uiAccessRep.Add(new AccessRightsOperationsUI()
                        {
                            AllowedAllOrganizations = false,
                            Id = Guid.NewGuid(),
                            OrganizationId = toAdd,
                            Permission = DomainEnum.GeneralDescriptions.ToCamelCase(),
                            Role = UserRoleEnum.Pete.ToString().ToLowerInvariant(),
                            RulesAll = (long) (PermisionEnum.Read | PermisionEnum.Create | PermisionEnum.Delete | PermisionEnum.Update | PermisionEnum.Publish),
                            RulesOwn = (long) (PermisionEnum.Read | PermisionEnum.Create | PermisionEnum.Delete | PermisionEnum.Update | PermisionEnum.Publish)
                        });
                    });
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                });
            }
        }
    }
}