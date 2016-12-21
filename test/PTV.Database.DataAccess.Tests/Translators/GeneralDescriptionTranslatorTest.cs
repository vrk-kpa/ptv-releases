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
using PTV.Database.DataAccess.Translators.GeneralDescription;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.ServiceManager;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators
{
    public class GeneralDescriptionTranslatorTest : TranslatorTestBase
    {
        private GeneralDescriptionResultsTranslator generalDescriptionTranslator;
        private StatutoryDescriptionStringTranslator statutoryDescriptionStringTranslator;
        private StatutoryServiceClassVmListItemTranslator statutoryServiceClassVmListItemTranslator;
        private TargetGroupVmListTranslator targetGroupVmListTranslator;
        private StatutoryServiceNameStringTranslator statutoryServiceNameStringTranslator;

        public GeneralDescriptionTranslatorTest()
        {
            var typesCache = SetupTypesCacheMock<NameType>(typesEnum: typeof(NameTypeEnum));
            SetupTypesCacheMock<DescriptionType>(typesCache, typeof(DescriptionTypeEnum));
            generalDescriptionTranslator = new GeneralDescriptionResultsTranslator(ResolveManager, TranslationPrimitives, typesCache.Object);
            statutoryDescriptionStringTranslator = new StatutoryDescriptionStringTranslator(ResolveManager, TranslationPrimitives);
            statutoryServiceClassVmListItemTranslator = new StatutoryServiceClassVmListItemTranslator(ResolveManager, TranslationPrimitives);
            targetGroupVmListTranslator = new TargetGroupVmListTranslator(ResolveManager, TranslationPrimitives);
            statutoryServiceNameStringTranslator = new StatutoryServiceNameStringTranslator(ResolveManager, TranslationPrimitives);
        }

        [Fact]
        public void TranslateGeneralDescriptionToVm()
        {
            var description = new StatutoryServiceGeneralDescription()
            {
                Descriptions = new List<StatutoryServiceDescription>()
                {
                    new StatutoryServiceDescription()
                    {
                        Description = "TestDesc123Short",
                        TypeId = DescriptionTypeEnum.ShortDescription.ToString().GetGuid()
                    },
                    new StatutoryServiceDescription()
                    {
                        Description = "TestDesc123Long",
                        TypeId = DescriptionTypeEnum.Description.ToString().GetGuid()
                    }
                },
                Names = new List<StatutoryServiceName>()
                {
                    new StatutoryServiceName()
                    {
                        Name = "Desc123Name",
                        TypeId = NameTypeEnum.Name.ToString().GetGuid()
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
                }
            };
            var toTranslate = new List<StatutoryServiceGeneralDescription>() {description};
            var translated = RunTranslationEntityToModelTest< StatutoryServiceGeneralDescription, VmGeneralDescriptionResults>(new List<ITranslator>() {generalDescriptionTranslator, statutoryDescriptionStringTranslator, statutoryServiceClassVmListItemTranslator, targetGroupVmListTranslator, statutoryServiceNameStringTranslator }, toTranslate);
            translated.Count.Should().Be(1);
            var target = translated.First();
            target.Name.Should().Be("Desc123Name");
            target.ServiceClasses.Count.Should().Be(2);
            target.ShortDescription.Should().Be("TestDesc123Short");
            target.Description.Should().Be("TestDesc123Long");
        }

    }
}
