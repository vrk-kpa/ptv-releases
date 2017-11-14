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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using Xunit;
using PTV.Domain.Model.Enums;
using PTV.Database.DataAccess.Interfaces.DbContext;


using PTV.Framework;
using PTV.Database.DataAccess.Tests.Translators;
using System;
using System.Reflection;
using System.Globalization;
using PTV.Database.Model.Models.Base;
using System.Collections;
using Moq;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Tests.Types.Translators
{
    public class TypeTranslatorTest : TranslatorTestBase
    {
        private LanguageCodeTranslator languageCodeTranslator;
        private IUnitOfWork unitOfWorkMock;

        public TypeTranslatorTest()
        {
            languageCodeTranslator = new LanguageCodeTranslator(ResolveManager, TranslationPrimitives);
            unitOfWorkMock = unitOfWorkMockSetup.Object as IUnitOfWork;
            RegisterDbSet(CreateCodeData<NameType>(typeof(NameTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<DescriptionType>(typeof(DescriptionTypeEnum)), unitOfWorkMockSetup);            
            RegisterDbSet(CreateCodeData<ProvisionType>(typeof(ProvisionTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<PublishingStatusType>(typeof(PublishingStatus)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<WebPageType>(typeof(WebPageTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<AttachmentType>(typeof(AttachmentTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<ExceptionHoursStatusType>(typeof(ExceptionHoursStatus)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<ServiceHourType>(typeof(ServiceHoursTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<ServiceChannelType>(typeof(ServiceChannelTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<ServiceChargeType>(typeof(ServiceChargeTypeEnum)), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for TypeTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(typeof(NameTypeCodeTranslator), typeof(NameType), typeof(NameTypeEnum))]
        [InlineData(typeof(DescriptionTypeCodeTranslator), typeof(DescriptionType), typeof(DescriptionTypeEnum))]
        [InlineData(typeof(ProvisionTypeCodeTranslator), typeof(ProvisionType), typeof(ProvisionTypeEnum))]
        [InlineData(typeof(PublishingStatusTypeTranslator), typeof(PublishingStatusType), typeof(PublishingStatus))]
        [InlineData(typeof(WebPageTypeCodeTranslator), typeof(WebPageType), typeof(WebPageTypeEnum))]
        [InlineData(typeof(AttachmentTypeCodeTranslator), typeof(AttachmentType), typeof(AttachmentTypeEnum))]
        [InlineData(typeof(ExceptionHoursStatusTypeCodeTranslator), typeof(ExceptionHoursStatusType), typeof(ExceptionHoursStatus))]
        [InlineData(typeof(ServiceHourTypeCodeTranslator), typeof(ServiceHourType), typeof(ServiceHoursTypeEnum))]
        [InlineData(typeof(ServiceChannelTypeCodeTranslator), typeof(ServiceChannelType), typeof(ServiceChannelTypeEnum))]
        [InlineData(typeof(ServiceChargeTypeCodeTranslator), typeof(ServiceChargeType), typeof(ServiceChargeTypeEnum))]
        public void TranslateTypeToEntity(Type translatorType, Type entityType, Type enumType)
        {
            var toTranslate = Enum.GetNames(enumType).ToList();
            var typeTranslator = translatorType.GetConstructor(new Type[] {typeof(IResolveManager), typeof(ITranslationPrimitives)}).Invoke(new object[] {ResolveManager, TranslationPrimitives}) as ITranslator;
            //var typeTranslator = (ITranslator)Activator.CreateInstance(translatorType, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { ResolveManager, TranslationPrimitives }, null);

            MethodInfo methodToEntity = GetType().GetMethod("RunTranslationModelToEntityTest", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo genericMethodToEntity = methodToEntity.MakeGenericMethod(new[] { typeof(string), entityType });
            var translators = new List<ITranslator> { typeTranslator };
            var translations = genericMethodToEntity.Invoke(this, new object[] { translators, toTranslate, unitOfWorkMock }) as IList;

            Assert.Equal(toTranslate.Count, translations.Count);
            for (int i = 0; i < translations.Count; i++)
            {
                Assert.Equal(toTranslate[i], ((TypeBase)translations[i]).Code);
            }

        }
    }
}
