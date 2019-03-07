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
using FluentAssertions;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Translators.Channels.V2;
using PTV.Database.DataAccess.Translators.Services.V2;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Services
{
    public class ServiceInputTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;
        private readonly TreeModelGenerator treeGenerator;
        private readonly ItemListModelGenerator itemListGenerator;
        private readonly TestConversion conversion;

        public ServiceInputTranslatorTest()
        {
            SetupEntityTreesCacheMock<TargetGroup>();
            translators = new List<object>
            {
                new ServiceSaveTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                RegisterTranslatorMock<ServiceVersioned, VmServiceBase>(),
                RegisterTranslatorMock<ServiceServiceClass, VmListItem>(),
                RegisterTranslatorMock<ServiceOntologyTerm, VmListItem>(),
                RegisterTranslatorMock<ServiceLifeEvent, VmListItem>(),
                RegisterTranslatorMock<ServiceIndustrialClass, VmListItem>(),
                RegisterTranslatorMock<ServiceKeyword, VmKeywordItem>(),
            };

            RegisterDbSet(CreateCodeData<PublishingStatusType>(typeof(PublishingStatus)), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceVersioned>(), unitOfWorkMockSetup);

            treeGenerator = new TreeModelGenerator();
            itemListGenerator = new ItemListModelGenerator();
            conversion = new TestConversion();
        }

        [Theory]
        [InlineData(0,0,0,0, 0,"")]
        [InlineData(1,1,1,1, 1, "")]
        [InlineData(8,15,5,13, 6, "KR1;KR2")]
        [InlineData(2,5,0,0,3, "KR1")]
        [InlineData(2,5,0,0,3, "KR2")]
        public void TranslateServiceStep2ToEntityTest(int lifeEventsTree, int ontologyTermsTree, int serviceClassesTree, int keywords, int industrialClasses, string targetGroups)
        {
            var model = new VmServiceInput
            {
                LifeEvents = itemListGenerator.Create<Guid>(lifeEventsTree),
                OntologyTerms = itemListGenerator.Create<Guid>(ontologyTermsTree),
                ServiceClasses = itemListGenerator.Create<Guid>(serviceClassesTree),
                IndustrialClasses = itemListGenerator.Create<Guid>(industrialClasses),
                Keywords = keywords > 0 ? new Dictionary<string, List<Guid>> {{ "fi", itemListGenerator.Create<Guid>(keywords) }} : new Dictionary<string, List<Guid>>(),
                
                TargetGroups = itemListGenerator.CreateList(targetGroups, tg => tg.GetGuid()).ToList()
            };
            
            var translation = RunTranslationModelToEntityTest<VmServiceInput, ServiceVersioned>(translators, model, unitOfWorkMockSetup.Object);
            CheckTranslation(model, translation, targetGroups);
        }

        private void CheckTranslation(VmServiceInput source, ServiceVersioned target, string targetGroups)
        {
            target.ServiceServiceClasses.Count.Should().Be(source.ServiceClasses.Count);
            target.ServiceOntologyTerms.Count.Should().Be(source.OntologyTerms.Count);
            target.ServiceKeywords.Count.Should().Be(source.Keywords.Sum(x => x.Value.Count));
            
            target.ServiceLifeEvents.Count.Should().Be(targetGroups.Contains("KR1") ? source.LifeEvents.Count : 0);
            target.ServiceIndustrialClasses.Count.Should().Be(targetGroups.Contains("KR2") ? source.IndustrialClasses.Count : 0);
        }
    }
}