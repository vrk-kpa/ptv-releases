﻿/**
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
using Microsoft.EntityFrameworkCore;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.Services;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework.ServiceManager;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Services
{
    public class ServiceDescriptionTranslatorTest : TranslatorTestBase
    {
        private ServiceDescriptionTranslator serviceDescriptionTranslator;
        private DescriptionTypeCodeTranslator descriptionTypeCodeTranslator;
        private LanguageCodeTranslator languageCodeTranslator;
        private IUnitOfWork unitOfWorkMock;


        public ServiceDescriptionTranslatorTest()
        {
            serviceDescriptionTranslator = new ServiceDescriptionTranslator(ResolveManager, TranslationPrimitives);
            descriptionTypeCodeTranslator = new DescriptionTypeCodeTranslator(ResolveManager, TranslationPrimitives);
            languageCodeTranslator = new LanguageCodeTranslator(ResolveManager, TranslationPrimitives);
            unitOfWorkMock = unitOfWorkMockSetup.Object;
        }

        /// <summary>
        /// test for ServiceDescriptionTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(DescriptionTypeEnum.Description)]
        [InlineData(DescriptionTypeEnum.ServiceUserInstruction)]
        [InlineData(DescriptionTypeEnum.ShortDescription)]
        public void TranslateServiceDescriptionToEntity(DescriptionTypeEnum descriptionType)
        {
            var vmDescription = CreateVmDescription(descriptionType);
            var toTranslate = new List<VmDescription>() { vmDescription };

            RegisterDbSet(CreateCodeData<DescriptionType>(typeof(DescriptionTypeEnum)), unitOfWorkMockSetup);
            string[] codes = {"fi", "sv", "en"}; 
            RegisterDbSet(codes.Select(type => new Language { Code = type }).ToList(), unitOfWorkMockSetup);

            var translations = RunTranslationModelToEntityTest<VmDescription, ServiceDescription>(new List<ITranslator> { serviceDescriptionTranslator, descriptionTypeCodeTranslator, languageCodeTranslator }, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(vmDescription.Description, translation.Description);
            Assert.Equal(vmDescription.TypeId, translation.TypeId);
        }

        private VmDescription CreateVmDescription(DescriptionTypeEnum descriptionType)
        {
            return new VmDescription()
            {
                Description = "desc",
                TypeId = Guid.NewGuid()//descriptionType
            };
        }

    }

}
