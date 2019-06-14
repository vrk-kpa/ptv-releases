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
using System.Threading.Tasks;
using FluentAssertions;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Database.DataAccess.Translators.Common.V2;
using PTV.Database.DataAccess.Translators.GeneralDescription.V2;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using PTV.Framework;
using PTV.Framework.ServiceManager;
using Xunit;
using GeneralDescriptionListItemTranslator = PTV.Database.DataAccess.Translators.GeneralDescription.GeneralDescriptionListItemTranslator;
using VmGeneralDescriptionListItem = PTV.Domain.Model.Models.VmGeneralDescriptionListItem;
using EntityDefinitionHelper = PTV.Database.DataAccess.Translators.Channels.V2.EntityDefinitionHelper;

namespace PTV.Database.DataAccess.Tests.Translators
{
    public class GeneralDescriptionTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;

        public GeneralDescriptionTranslatorTest()
        {
            SetupTypesCacheMock<DescriptionType>(typeof(DescriptionTypeEnum));
            SetupTypesCacheMock<PublishingStatusType>(typeof(PublishingStatus));
            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));
            SetupTypesCacheMock<DescriptionType>(typeof(DescriptionTypeEnum));
            translators = new List<object>
            {
                new GeneralDescriptionListItemTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new StatutoryDescriptionStringTranslator(ResolveManager, TranslationPrimitives),
                new StatutoryServiceClassVmListItemTranslator(ResolveManager, TranslationPrimitives),
                new TargetGroupVmListTranslator(ResolveManager, TranslationPrimitives),
                new GeneralDescriptionHeaderTranslator(ResolveManager, TranslationPrimitives),
                new NameTranslator(ResolveManager, TranslationPrimitives),
                new DescriptionTranslator(ResolveManager, TranslationPrimitives),
                new ChargeTypeTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new EntityHeaderTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new StatutoryServiceNameStringTranslator(ResolveManager, TranslationPrimitives),
                new GeneralDescriptionVersionedTranslator(ResolveManager, TranslationPrimitives, CacheManager, new EntityDefinitionHelper(CacheManager)),
                new VersionTranslator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock<ILanguageAvailability, VmLanguageAvailabilityInfo>(model => new GeneralDescriptionLanguageAvailability())
            };
        }

        [Fact]
        public void TranslateGeneralDescriptionToVm()
        {
            var description = new StatutoryServiceGeneralDescriptionVersioned()
            {
                Descriptions = new List<StatutoryServiceDescription>()
                {
                    new StatutoryServiceDescription()
                    {
                        Description = "TestDesc123Short",
                        LocalizationId = CacheManager.LanguageCache.Get("fi"),
                        TypeId = DescriptionTypeEnum.ShortDescription.ToString().GetGuid()
                    },
                    new StatutoryServiceDescription()
                    {
                        Description = "TestDesc123Long",
                        LocalizationId = CacheManager.LanguageCache.Get("fi"),
                        TypeId = DescriptionTypeEnum.Description.ToString().GetGuid()
                    }
                },
                Names = new List<StatutoryServiceName>()
                {
                    new StatutoryServiceName()
                    {
                        Name = "Desc123Name",
                        LocalizationId = CacheManager.LanguageCache.Get("fi"),
                        TypeId = NameTypeEnum.Name.ToString().GetGuid(),
                    }
                },
                ServiceClasses = new List<StatutoryServiceServiceClass>()
                {
                    new StatutoryServiceServiceClass()
                    {
                        ServiceClass = new ServiceClass()
                        {
                            Label = "SomeServiceClass"
                        }
                    },
                    new StatutoryServiceServiceClass()
                    {
                        ServiceClass = new ServiceClass()
                        {
                            Label = "AnotherServiceClass"
                        }
                    }
                },
                TargetGroups = new List<StatutoryServiceTargetGroup>()
                {
                    new StatutoryServiceTargetGroup()
                    {
                       TargetGroup = new TargetGroup()
                       {
                           Label = "SomeTargetGroup"
                       }
                    }
                },
                LanguageAvailabilities = new List<GeneralDescriptionLanguageAvailability>
                {
                    new GeneralDescriptionLanguageAvailability
                    {
                        LanguageId = CacheManager.LanguageCache.Get("fi"),
                        StatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published),
                        // todo: replace it by language order cache
                        Language = new Language { OrderNumber = 5 }
                    }
                }
            };
            var target = RunTranslationEntityToModelTest<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescriptionListItem>(translators, description);
            target.Name.Count.Should().Be(1);
            target.Name.Should().ContainValue("Desc123Name");
            target.ServiceClasses.Count.Should().Be(2);
            target.ShortDescription.Count.Should().Be(1);
            target.ShortDescription.Should().ContainValue("TestDesc123Short");
            target.Description.Should().ContainValue("TestDesc123Long");
        }
        
        [Fact]
        public void TranslateGeneralDescriptionBaseToVmBase()
        {
            var guid = Guid.NewGuid();
            
            var description = new StatutoryServiceGeneralDescriptionVersioned()
            {
                UnificRootId = guid,
                Id = guid,
                TypeId = guid,
                GeneralDescriptionTypeId = guid,
                Descriptions = new List<StatutoryServiceDescription>()
                {
                    new StatutoryServiceDescription()
                    {
                        Description = "TestDesc123Short",
                        LocalizationId = CacheManager.LanguageCache.Get("fi"),
                        TypeId = DescriptionTypeEnum.ShortDescription.ToString().GetGuid()
                    },
                    new StatutoryServiceDescription()
                    {
                        Description = "TestDesc123Long",
                        LocalizationId = CacheManager.LanguageCache.Get("fi"),
                        TypeId = DescriptionTypeEnum.Description.ToString().GetGuid()
                    },
                    new StatutoryServiceDescription()
                    {
                        Description = "TestDescBck123Long",
                        LocalizationId = CacheManager.LanguageCache.Get("fi"),
                        TypeId = DescriptionTypeEnum.BackgroundDescription.ToString().GetGuid()
                    },
                    new StatutoryServiceDescription()
                    {
                        Description = "TestDescDeadLine123Long",
                        LocalizationId = CacheManager.LanguageCache.Get("fi"),
                        TypeId = DescriptionTypeEnum.DeadLineAdditionalInfo.ToString().GetGuid()
                    },
                    new StatutoryServiceDescription()
                    {
                        Description = "TestDescProcess123Long",
                        LocalizationId = CacheManager.LanguageCache.Get("fi"),
                        TypeId = DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString().GetGuid()
                    },
                    new StatutoryServiceDescription()
                    {
                        Description = "TestDescSUI123Long",
                        LocalizationId = CacheManager.LanguageCache.Get("fi"),
                        TypeId = DescriptionTypeEnum.ServiceUserInstruction.ToString().GetGuid()
                    },
                    new StatutoryServiceDescription()
                    {
                        Description = "TestDescValidity123Long",
                        LocalizationId = CacheManager.LanguageCache.Get("fi"),
                        TypeId = DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString().GetGuid()
                    },
                    new StatutoryServiceDescription()
                    {
                        Description = "TestDescUseOfGd123Long",
                        LocalizationId = CacheManager.LanguageCache.Get("fi"),
                        TypeId = DescriptionTypeEnum.GeneralDescriptionTypeAdditionalInformation.ToString().GetGuid()
                    }
                },
                TargetGroups = new List<StatutoryServiceTargetGroup>()
                {
                    new StatutoryServiceTargetGroup()
                    {
                       TargetGroup = new TargetGroup()
                       {
                           Label = "SomeTargetGroup"
                       }
                    }
                }
            };
            var target = RunTranslationEntityToModelTest<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescriptionBase>(translators, description);

            target.UnificRootId.Should().Be(guid);
            target.Id.Should().Be(guid);
            target.ServiceType.Should().Be(guid);
            target.GeneralDescriptionType.Should().Be(guid);
            target.ShortDescription.Count.Should().Be(1);
            target.ShortDescription.Should().ContainValue("TestDesc123Short");
            target.Description.Should().ContainValue("TestDesc123Long");
            target.GeneralDescriptionTypeAdditionalInformation.Should().ContainValue("TestDescUseOfGd123Long");
            target.BackgroundDescription.Should().ContainValue("TestDescBck123Long");
            target.DeadLineInformation.Should().ContainValue("TestDescDeadLine123Long");
            target.ProcessingTimeInformation.Should().ContainValue("TestDescProcess123Long");
            target.UserInstruction.Should().ContainValue("TestDescSUI123Long");
            target.ValidityTimeInformation.Should().ContainValue("TestDescValidity123Long");
        }
    }
}
