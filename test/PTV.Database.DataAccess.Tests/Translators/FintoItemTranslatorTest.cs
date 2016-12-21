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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Tests.Translators
{
    public class FintoItemTranslatorTest : TranslatorTestBase
    {
        private IReadOnlyList<ITranslator> translators;
        private IUnitOfWork unitOfWorkMock;

        public FintoItemTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            translators = new List<ITranslator>
            {
                new FintoLifeEventNameTranslator(ResolveManager, TranslationPrimitives),
                new FintoLifeEventTranslator(ResolveManager, TranslationPrimitives),
                new FintoTargetGroupNameTranslator(ResolveManager, TranslationPrimitives),
                new FintoTargetGroupTranslator(ResolveManager, TranslationPrimitives),
                new FintoServiceClassNameTranslator(ResolveManager, TranslationPrimitives),
                new FintoServiceClassTranslator(ResolveManager, TranslationPrimitives),
                new FintoOntologyTermNameTranslator(ResolveManager, TranslationPrimitives),
                new FintoOntologyTermTranslator(ResolveManager, TranslationPrimitives),
                new LanguageCodeTranslator(ResolveManager, TranslationPrimitives)
            };

            RegisterDbSet(CreateCodeData<Language>(typeof(LanguageCode)), unitOfWorkMockSetup);
            RegisterDbSet(new List<TargetGroup>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<LifeEvent>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceClass>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<OntologyTerm>(), unitOfWorkMockSetup);

        }


        /// <summary>
        /// test for FintoLifeEventTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateFintoLifeEventsToEntity()
        {
            RunTestVmToEntity<VmFintoJsonItem, LifeEvent, LifeEventName>(translators, unitOfWorkMock);
        }

        /// <summary>
        /// test for FintoOntologyTermTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateFintoOntologyTermsToEntity()
        {
            RunTestVmToEntity<VmFintoJsonItem, OntologyTerm, OntologyTermName>(translators, unitOfWorkMock);
        }

        /// <summary>
        /// test for FintoServiceClassTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateFintoServiceClassesToEntity()
        {
            RunTestVmToEntity<VmFintoJsonItem, ServiceClass, ServiceClassName>(translators, unitOfWorkMock);
        }

        /// <summary>
        /// test for FintoTargetGroupTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateFintoTargetGroupsToEntity()
        {
            RunTestVmToEntity<VmFintoJsonItem, TargetGroup, TargetGroupName>(translators, unitOfWorkMock);
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

        private void RunTestVmToEntity<TVModel, TEntity, TName>(IReadOnlyCollection<ITranslator> translators, IUnitOfWork ofWorkMock) where TEntity : FintoItemBase, IFintoItemNames<TName> where TVModel : VmFintoJsonItem where TName : NameBase
        {
            var fintoItem = CreateFintoItem<TVModel>();
            var toTranslate = new List<TVModel> { fintoItem };
            var translations = RunTranslationModelToEntityTest<TVModel, TEntity>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            translation.Uri.Should().Be(fintoItem.Id);
            translation.Label.Should().Be(fintoItem.Label);
            translation.Code.Should().Be(fintoItem.Notation);
            translation.OntologyType.Should().Be(fintoItem.OntologyType);
            AssertNames(fintoItem, translation.Names);
        }

        private TVModel CreateFintoItem<TVModel>() where TVModel : VmFintoJsonItem
        {
            var instance = Activator.CreateInstance<TVModel>();
            instance.Id = "Id";
            instance.Label = "Label";
            instance.Notation = "Notification";
            instance.OntologyType = "OntologyType";
            instance.Finnish = "Finnish";
            instance.Narrower = new List<VmFintoJsonItem>();
            return instance;
        }

        private void AssertNames(VmFintoJsonItem finto, IEnumerable<IName> names)
        {
            var nameList = names.ToList();
            nameList.Count.Should().BeGreaterThan(0);
            foreach (var name in nameList)
            {
                name.Name.Should().Be(finto.Finnish);
                name.LocalizationId.Should().NotBe(Guid.Empty);
                //name.Localization.Code.Should().Be(LanguageCode.fi.ToString());
            }
        }
    }
}
