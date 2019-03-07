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
using PTV.Database.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Xunit;
using FluentAssertions;
using PTV.Database.DataAccess.Repositories;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Views;

namespace PTV.Database.DataAccess.Tests.Repositories
{
    public class RepositoryGenerationTest
    {
        [Fact]
        public void CheckAllRepositoriesAreGenerated()
        {
            //            var type = typeof (PTV.Database.Model.Models.Base.EntityBase);
            var type = typeof(ServiceVersioned);
            var viewType = typeof(VTasksConfiguration);
            var repositoryType = typeof(Repository);
            var viewRepositoryType = typeof(VRepository);
            const string repositoryName = "Repository";
            var types = type.GetTypeInfo().Assembly.DefinedTypes
                .Where(x => x.IsClass && (x.Namespace == type.Namespace || x.Namespace == viewType.Namespace))
                .ToDictionary(x => x.Name);
            var repositories = repositoryType
                .GetTypeInfo().Assembly.DefinedTypes
                .Where(x => x.IsClass && x.Namespace == repositoryType.Namespace && x.Name.EndsWith(repositoryName) && x != repositoryType.GetTypeInfo() && x != viewRepositoryType.GetTypeInfo())
                .ToDictionary(x => x.Name);
            StringBuilder errors = new StringBuilder();
            int errorCounts = 0;
            foreach (var typeInfo in types.Values)
            {
                if(!repositories.ContainsKey(typeInfo.Name + repositoryName))
                {
                    var error = $"Repository is missing for model type {typeInfo.Name}.";
                    errors.AppendLine(error);
                    errorCounts++;
                }
            }
            foreach (var repository in repositories.Values)
            {
                if (!types.ContainsKey(repository.Name.Replace(repositoryName, string.Empty)))
                {
                    var error = $"Model is missing for repository {repository.Name}.";
                    errors.AppendLine(error);
                    errorCounts++;
                }
            }
            errorCounts.Should().Be(0, errors.ToString());
            repositories.Count.Should().Be(types.Count);
        }

        [Fact]
        public void CheckAllRepositoryInterfacesAreGenerated()
        {
            //            var type = typeof (PTV.Database.Model.Models.Base.EntityBase);
            var interfaceRepository = typeof(IVRepository);
            var repositoryType = typeof(Repository);
            var viewRepositoryType = typeof(VRepository);
            const string repositoryName = "Repository";
            var interfaces = interfaceRepository.GetTypeInfo().Assembly.DefinedTypes
                .Where(x => x.IsInterface && x.Namespace == interfaceRepository.Namespace && x.Name.EndsWith(repositoryName))
                .ToDictionary(x => x.Name);
            var repositories = repositoryType.GetTypeInfo().Assembly.DefinedTypes
                .Where(x => x.IsClass && x.Namespace == repositoryType.Namespace && x.Name.EndsWith(repositoryName) /*&& x != repositoryType.GetTypeInfo() && x != viewRepositoryType.GetTypeInfo()*/)
                .ToDictionary(x => x.Name);
            StringBuilder errors = new StringBuilder();
            int errorConunts = 0;
            foreach (var typeInfo in interfaces.Values)
            {
                if (!repositories.ContainsKey(typeInfo.Name.Substring(1, typeInfo.Name.Length - 1)))
                {
                    var error = $"Repository is missing for interface {typeInfo.Name}.";
                    errors.AppendLine(error);
                    errorConunts++;
                }
            }
            foreach (var repository in repositories.Values)
            {
                if (!interfaces.ContainsKey("I" + repository.Name))
                {
                    var error = $"Interface is missing for repository {repository.Name}.";
                    errors.AppendLine(error);
                    errorConunts++;
                }
            }
            errorConunts.Should().Be(0, errors.ToString());
            repositories.Count.Should().Be(interfaces.Count);
        }
    }
}
