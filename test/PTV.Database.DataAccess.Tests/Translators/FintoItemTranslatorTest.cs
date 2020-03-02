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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using Xunit;
using System;
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Finto;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Import;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Tests.Translators
{
    public class FintoItemTranslatorTest : TranslatorTestBase
    {
        private IReadOnlyList<ITranslator> translators;
        private IUnitOfWork unitOfWorkMock;

        public FintoItemTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            translators = new List<ITranslator>
            {
                new FintoLifeEventNameTranslator(ResolveManager, TranslationPrimitives, CacheManager.LanguageCache),
                new FintoLifeEventTranslator(ResolveManager, TranslationPrimitives),
                new FintoTargetGroupNameTranslator(ResolveManager, TranslationPrimitives, CacheManager.LanguageCache),
                new FintoTargetGroupTranslator(ResolveManager, TranslationPrimitives),
                new FintoServiceClassNameTranslator(ResolveManager, TranslationPrimitives, CacheManager.LanguageCache),
                new FintoServiceClassTranslator(ResolveManager, TranslationPrimitives),
                new FintoOntologyTermNameTranslator(ResolveManager, TranslationPrimitives, CacheManager.LanguageCache),
                new FintoOntologyTermTranslator(ResolveManager, TranslationPrimitives),
                new LanguageCodeTranslator(ResolveManager, TranslationPrimitives)
            };
            string[] codes = {"fi", "sv", "en"};
            RegisterDbSet(codes.Select(x => new Language { Code = x }).ToList(), unitOfWorkMockSetup);
            RegisterDbSet(new List<TargetGroup>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<TargetGroupName>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<LifeEvent>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<LifeEventName>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceClass>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceClassName>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<OntologyTerm>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<OntologyTermName>(), unitOfWorkMockSetup);

        }


        /// <summary>
        /// test for FintoLifeEventTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateFintoLifeEventsToEntity()
        {
            RunTestVmToEntity<VmServiceViewsJsonItem, LifeEvent, LifeEventName>(translators);
        }

        /// <summary>
        /// test for FintoOntologyTermTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateFintoOntologyTermsToEntity()
        {
            RunTestVmToEntity<VmServiceViewsJsonItem, OntologyTerm, OntologyTermName>(translators);
        }

        /// <summary>
        /// test for FintoServiceClassTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateFintoServiceClassesToEntity()
        {
            RunTestVmToEntity<VmServiceViewsJsonItem, ServiceClass, ServiceClassName>(translators);
        }

        /// <summary>
        /// test for FintoTargetGroupTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateFintoTargetGroupsToEntity()
        {
            RunTestVmToEntity<VmServiceViewsJsonItem, TargetGroup, TargetGroupName>(translators);
        }

        ///// <summary>
        ///// test for FintoOrganizationTypeTranslator vm - > entity
        ///// </summary>
        //[Fact]
        //public void TranslateFintoOrganizationTypesToEntity()
        //{
        //    var nameTranlator = new FintoOrganizationTypeNameTranslator(ResolveManager, TranslationPrimitives);
        //    var translator = new FintoOrganizationTypeTranslator(ResolveManager, TranslationPrimitives);
        //    RunTestVmToEntity<VmOrganizationType, OrganizationType>(new List<ITranslator>{ translator, nameTranlator } , (fintoItem, entity) => AssertNames(fintoItem, entity.Names));
        //}

        private void RunTestVmToEntity<TVModel, TEntity, TName>(IReadOnlyCollection<ITranslator> translators) where TEntity : FintoItemBase, IFintoItemNames<TName> where TVModel : VmServiceViewsJsonItem where TName : NameBase
        {
            var fintoItem = CreateFintoItem<TVModel>();
            var toTranslate = new List<TVModel> { fintoItem };
            var translations = RunTranslationModelToEntityTest<TVModel, TEntity>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            translation.Uri.Should().Be(fintoItem.Id);
            translation.Label.Should().Be(fintoItem.Label.TryGet("fi"));
            translation.Code.Should().Be(fintoItem.Notation);
            translation.OntologyType.Should().Be(fintoItem.ConceptType);
            AssertNames(fintoItem, translation.Names);
        }

        private TVModel CreateFintoItem<TVModel>() where TVModel : VmServiceViewsJsonItem
        {
            var instance = Activator.CreateInstance<TVModel>();
            instance.Id = "Id";
            instance.Label = new Dictionary<string, string> { { "fi", "fi" } };
            instance.Notation = "Notification";
            instance.ConceptType = "OntologyType";
            instance.NarrowerURIs = new List<string>();
            instance.BroaderURIs = new List<string>();
            return instance;
        }

        private void AssertNames(VmServiceViewsJsonItem finto, IEnumerable<IName> names)
        {
            var nameList = names.ToList();
            nameList.Count.Should().BeGreaterThan(0);
            foreach (var name in nameList)
            {
                name.Name.Should().Be(finto.Label.Select(x => x.Value).FirstOrDefault());
                name.LocalizationId.Should().NotBe(Guid.Empty);
                //name.Localization.Code.Should().Be("fi".ToString());
            }
        }
    }
}
