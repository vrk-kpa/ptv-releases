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
using System.Collections.ObjectModel;
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.Types.Json;
using PTV.Domain.Model.Models.Import;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Tests.Types.Translators
{
    public class TypeTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;

        public TypeTranslatorTest()
        {
            translators = new List<object>()
            {
                new NameTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new PhoneNumberTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new DescriptionTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new AddressCharacterJsonTranslator(ResolveManager, TranslationPrimitives),
                new AddressTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new WebPageTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new PublishingStatusTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new ServiceChannelTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new ProvisionTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new ServiceChargeTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new ServiceHourTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new ExceptionHoursStatusTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new AttachmentTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new PrintableFormChannelUrlTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new ServiceTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new CoordinateTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new AppEnvironmentDataTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new AreaInformationTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new AreaTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new ServiceChannelConnectionTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new ServiceFundingTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new AccessRightTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new ExtraTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new ExtraSubTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new TranslationStateTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new GeneralDescriptionTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new AccessibilityClassificationLevelTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                new WcagLevelTypeJsonTranslator(ResolveManager, TranslationPrimitives),
                
                new NameTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new PhoneNumberTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new DescriptionTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new AddressCharacterNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new AddressTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new WebPageTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new PublishingStatusTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new ServiceChannelTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new ProvisionTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new ServiceChargeTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new ServiceHourTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new ExceptionHoursStatusTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new AttachmentTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new PrintableFormChannelUrlTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new ServiceTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new CoordinateTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new AppEnvironmentDataTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new AreaInformationTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new AreaTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new ServiceChannelConnectionTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new ServiceFundingTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new AccessRightNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new ExtraTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new ExtraSubTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new TranslationStateNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new GeneralDescriptionTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new AccessibilityClassificationLevelTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new WcagLevelTypeNameJsonTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                
            };
            
            
        }

        [Fact]
        public void TranslateTypeToEntity()
        {
            TranslateType<NameType, NameTypeName>();
            TranslateType<PhoneNumberType, PhoneNumberTypeName>();
            TranslateType<DescriptionType, DescriptionTypeName>();
            TranslateType<AddressCharacter, AddressCharacterName>();
            TranslateType<AddressType, AddressTypeName>();
            TranslateType<WebPageType, WebPageTypeName>();
            TranslateType<PublishingStatusType, PublishingStatusTypeName>();
            TranslateType<ServiceChannelType, ServiceChannelTypeName>();
            TranslateType<ProvisionType, ProvisionTypeName>();
            TranslateType<ServiceChargeType, ServiceChargeTypeName>();
            TranslateType<ServiceHourType, ServiceHourTypeName>();
            TranslateType<ExceptionHoursStatusType, ExceptionHoursStatusTypeName>();
            TranslateType<AttachmentType, AttachmentTypeName>();
            TranslateType<PrintableFormChannelUrlType, PrintableFormChannelUrlTypeName>();
            TranslateType<ServiceType, ServiceTypeName>();
            TranslateType<CoordinateType, CoordinateTypeName>();
            TranslateType<AppEnvironmentDataType, AppEnvironmentDataTypeName>();
            TranslateType<AreaInformationType, AreaInformationTypeName>();
            TranslateType<AreaType, AreaTypeName>();
            TranslateType<ServiceChannelConnectionType, ServiceChannelConnectionTypeName>();
            TranslateType<ServiceFundingType, ServiceFundingTypeName>();
            TranslateType<AccessRightType, AccessRightName>();
            TranslateType<ExtraType, ExtraTypeName>();
            TranslateType<ExtraSubType, ExtraSubTypeName>();
            TranslateType<TranslationStateType, TranslationStateTypeName>();
            TranslateType<GeneralDescriptionType, GeneralDescriptionTypeName>();
            TranslateType<AccessibilityClassificationLevelType, AccessibilityClassificationLevelTypeName>();
            TranslateType<WcagLevelType, WcagLevelTypeName>();
        }

        private void TranslateType<TEntity, TNameEntity>() where TEntity : TypeBase<TNameEntity>, new() where TNameEntity : NameBase
        {
            RegisterDbSet(CreateCodeData<TEntity>(typeof(NameTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(new List<TNameEntity>(), unitOfWorkMockSetup);

            var model = CreateModel();
            var entity = RunTranslationModelToEntityTest<VmJsonTypeItem, TEntity>(translators, model, unitOfWorkMockSetup.Object);
            entity.Should().NotBeNull();
            entity.Code.Should().Be(model.Code);
            entity.Names.Count.Should().Be(model.Names.Count);
            var namesDic = model.Names.ToDictionary(x => x.Language.GetGuid());
            entity.Names.ForEach(name =>
            {
                "sv".GetGuid().Should().Be(CacheManager.LanguageCache.Get("sv"));
                name.LocalizationId.Should().NotBeEmpty();
                namesDic.ContainsKey(name.LocalizationId).Should().BeTrue($"{typeof(TEntity)} - {name.LocalizationId} is not found in {string.Join(", ", namesDic.Keys)}");
                name.Name.Should().Be(namesDic[name.LocalizationId].Name);
            });
        }

        private VmJsonTypeItem CreateModel()
        {
            return new VmJsonTypeItem
            {
                Code = "code",
                Names = new List<VmJsonTypeName>
                {
                    new VmJsonTypeName { Language = "sv", Name = "name", TypeId = "type".GetGuid()}
                },
                OrderNumber = 45,
                PriorityFallback = 21
            };
        }
        
//        [Fact]
//        public void TranslateTypeToModel()
//        {
//            TranslateModel<NameType, NameTypeName>();
//            TranslateModel<PhoneNumberType, PhoneNumberTypeName>();
//            TranslateModel<DescriptionType, DescriptionTypeName>();
//            TranslateModel<AddressCharacter, AddressCharacterName>();
//            TranslateModel<AddressType, AddressTypeName>();
//            TranslateModel<WebPageType, WebPageTypeName>();
//            TranslateModel<PublishingStatusType, PublishingStatusTypeName>();
//            TranslateModel<ServiceChannelType, ServiceChannelTypeName>();
//            TranslateModel<ProvisionType, ProvisionTypeName>();
//            TranslateModel<ServiceChargeType, ServiceChargeTypeName>();
//            TranslateModel<ServiceHourType, ServiceHourTypeName>();
//            TranslateModel<ExceptionHoursStatusType, ExceptionHoursStatusTypeName>();
//            TranslateModel<AttachmentType, AttachmentTypeName>();
//            TranslateModel<PrintableFormChannelUrlType, PrintableFormChannelUrlTypeName>();
//            TranslateModel<ServiceType, ServiceTypeName>();
//            TranslateModel<CoordinateType, CoordinateTypeName>();
//            TranslateModel<AppEnvironmentDataType, AppEnvironmentDataTypeName>();
//            TranslateModel<AreaInformationType, AreaInformationTypeName>();
//            TranslateModel<AreaType, AreaTypeName>();
//            TranslateModel<ServiceChannelConnectionType, ServiceChannelConnectionTypeName>();
//            TranslateModel<ServiceFundingType, ServiceFundingTypeName>();
//            TranslateModel<AccessRightType, AccessRightName>();
//            TranslateModel<ExtraType, ExtraTypeName>();
//            TranslateType<ExtraSubType, ExtraSubTypeName>();
//            TranslateModel<TranslationStateType, TranslationStateTypeName>();
//            TranslateModel<GeneralDescriptionType, GeneralDescriptionTypeName>();
//            TranslateModel<AccessibilityClassificationLevelType, AccessibilityClassificationLevelTypeName>();
//            TranslateModel<WcagLevelType, WcagLevelTypeName>();
//        }
        
        private void TranslateModel<TEntity, TNameEntity>() where TEntity : TypeBase<TNameEntity>, new() where TNameEntity : NameBase, new()
        {
            RegisterDbSet(CreateCodeData<TEntity>(typeof(NameTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(new List<TNameEntity>(), unitOfWorkMockSetup);

            var entity = CreateEntity<TEntity, TNameEntity>();
            var model = RunTranslationEntityToModelTest<TEntity, VmJsonTypeItem>(translators, entity);
            model.Should().NotBeNull();
            model.Code.Should().Be(entity.Code);
            model.Names.Count.Should().Be(entity.Names.Count);
            var namesDic = entity.Names.ToDictionary(x => x.LocalizationId);
            model.Names.ForEach(name =>
            {
                "sv".GetGuid().Should().Be(CacheManager.LanguageCache.Get("sv"));
                name.Language.Should().NotBeEmpty();
                namesDic.ContainsKey(name.Language.GetGuid()).Should().BeTrue();
                name.Name.Should().Be(namesDic[name.Language.GetGuid()].Name);
            });
        }

        private TEntity CreateEntity<TEntity, TNameEntity>() where TEntity : TypeBase<TNameEntity>, new() where TNameEntity : NameBase, new()
        {
            return new TEntity
            {
                Id = "TestEntity".GetGuid(),
                Code = "code",
                Names = new List<TNameEntity>
                {
                    new TNameEntity
                    {
                        Name = "name"
                    }
                }
                
            };
        } 
    }
}
