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
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Import;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services.Import
{
    [RegisterService(typeof(IUserRolesImport), RegisterType.Transient)]
    internal class UserRolesImport : IUserRolesImport
    {
        private readonly IContextManager contextManager;
        private IHostingEnvironment environment;
        private string ConfigurationFile = "UserRoles.json";
        
        public UserRolesImport(IContextManager contextManager, IHostingEnvironment environment)
        {
            this.contextManager = contextManager;
            this.environment = environment;
        }

        public void UpdateUserRoles()
        {
            var path = environment.GetFilePath(Path.Combine("..", @"PTV.Database.DataAccess", "Services", "Security"), ConfigurationFile);
            Console.WriteLine($"Importing file '{path}'");
            var file = File.ReadAllText(path);
            var definedRoles = JsonConvert.DeserializeObject<Dictionary<string, VmRoleInfo>>(file);

            var roles = definedRoles.SelectMany(i => i.Value.Permisions.Select(j => new { i.Value.AllowedAllOrganizations, Role = i.Key, Permission = j.Value.Code, j.Value.RulesAll, j.Value.RulesOwn, i.Value.OrganizationId})).ToList();
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IAccessRightsOperationsUIRepository>();
                
                var toUpdate = rep.All().Where(i => roles.Any(j => j.Role == i.Role && j.OrganizationId == i.OrganizationId && j.Permission == i.Permission)).ToList();
                toUpdate.ForEach(entity =>
                {
                    var newData = roles.First(j => j.Role == entity.Role && (j.OrganizationId == entity.OrganizationId) && j.Permission == entity.Permission);
                    entity.AllowedAllOrganizations = newData.AllowedAllOrganizations;
                    entity.RulesAll = (long)newData.RulesAll;
                    entity.RulesOwn = (long)newData.RulesOwn;
                });
                var toAdd = roles.Where(i => !toUpdate.Any(j => j.Role == i.Role && j.OrganizationId == i.OrganizationId && j.Permission == i.Permission)).ToList();
                toAdd.ForEach(i =>
                {
                    rep.Add(new AccessRightsOperationsUI()
                    {
                        OrganizationId = i.OrganizationId,
                        AllowedAllOrganizations = i.AllowedAllOrganizations,
                        RulesAll = (long)i.RulesAll,
                        RulesOwn = (long)i.RulesOwn,
                        Id = Guid.NewGuid(),
                        Permission = i.Permission,
                        Role = i.Role
                    });
                });
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });

        }
        
    }
}