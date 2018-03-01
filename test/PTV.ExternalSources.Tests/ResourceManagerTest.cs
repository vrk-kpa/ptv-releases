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
using FluentAssertions;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using Xunit;
using PTV.ExternalSources.Resources.Finto;
using PTV.ExternalSources.Resources.Types;
using PTV.Framework.Enums;

namespace PTV.ExternalSources.Tests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class ResourceManagerTest
    {
        public ResourceManagerTest()
        {
        }

        [Theory]
        [InlineData(JsonResources.LifeSituations)]
        [InlineData(JsonResources.TargetGroups)]
        [InlineData(JsonResources.ServiceClasses)]
        //[InlineData(JsonResources.Ontologies)]
        [InlineData(JsonResources.MunicipalityOrganizations)]
        [InlineData(JsonResources.PostalCode)]
        public void JsonLoadTest(JsonResources resource)
        {
            var resourceManager = new ResourceManager();
            var json = resourceManager.GetJsonResource(resource);
            Assert.NotNull(json);
        }

//        [Fact]
        public void JsonLoadDeserializedIndustrialClassesTest()
        {
            var resourceManager = new ResourceLoader();
            //            MethodInfo method = resourceManager.GetType().GetMethod("GetDesrializedJsonResource");
            //            MethodInfo generic = method.MakeGenericMethod(customNonFintoType);
            //            IList json = generic.Invoke(resourceManager, new object[] { resource }) as IList;
            var json = resourceManager.GetDeserializedResource<List<VmIndustrialClassJsonItem>>(new FintoItemImportDefinition { Resource = FintoItemImportDefinition.Resources.IndustrialClasses });
            Assert.NotNull(json);
            json.Count.Should().Be(1805);
            var finto = FillChildren(json);

            //CountData(finto).Should().Be(expectedCountAll);

            //File.WriteAllText($@"D:\IndustrialClassesTree.json", JsonConvert.SerializeObject(finto));
        }

        [Theory]
        [InlineData(FintoItemImportDefinition.Resources.LifeSituations, 14, 19, 19)]
        [InlineData(FintoItemImportDefinition.Resources.TargetGroups, 3, 17, 17)]
//        [InlineData(FintoItemImportDefinition.Resources.ServiceClasses, 27, 212, 0)]
        [InlineData(FintoItemImportDefinition.Resources.ServiceClasses, 28, 228, 211)]
        [InlineData(FintoItemImportDefinition.Resources.OntologyAll, 3, 37770, 100)]
        public void JsonLoadDeserializedTest(FintoItemImportDefinition.Resources resource, int expectedCountMain, int expectedCountAll, int replacedCount)
        {
            var resourceManager = new ResourceLoader();
            var json = resourceManager.GetDeserializedResource<List<VmServiceViewsJsonItem>>(new FintoItemImportDefinition { Resource = resource});
            Assert.NotNull(json);
            json.Count(x => (x.BroaderURIs == null || x.BroaderURIs.Count == 0) && string.IsNullOrEmpty(x.ReplacedBy)).Should().Be(expectedCountMain);
//            FillChildren(json);
            var notReplaced = json.Count(x => string.IsNullOrEmpty(x.ReplacedBy));
            notReplaced.Should().Be(expectedCountAll);
            var replaced = json.Count(x => !string.IsNullOrEmpty(x.ReplacedBy));
            replaced.Should().Be(replacedCount);
            json.Count.Should().Be(replaced + notReplaced);

            //            File.WriteAllText($@"D:\{resource}_update.json", JsonConvert.SerializeObject(json));
        }

        [Theory]
        [InlineData(FintoItemImportDefinition.Resources.LifeSituations,0,0)]
        [InlineData(FintoItemImportDefinition.Resources.TargetGroups,0,0)]
        [InlineData(FintoItemImportDefinition.Resources.ServiceClasses,1,0)]
        [InlineData(FintoItemImportDefinition.Resources.OntologyAll,3,2)]
        public void JsonLoadDeserializedMigrationTest(FintoItemImportDefinition.Resources resource, int replaceMore, int notFoundCount)
        {
            var resourceManager = new ResourceLoader();
            var json = resourceManager.GetDeserializedResource<List<VmServiceViewsJsonItem>>(new FintoItemImportDefinition { Resource = resource});
            Assert.NotNull(json);
            var ids = json.GroupBy(x => x.Id).ToDictionary(x => x.Key);

            var duplicated = ids.Where(x => x.Value.Count() > 1).Select(x => x.Key).ToList();
            duplicated.Count.Should().Be(0, "Duplicated keys: " + String.Join(", ", duplicated));

            var replaceList = json.Where(x => !string.IsNullOrEmpty(x.ReplacedBy)).GroupBy(x => x.ReplacedBy).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            replaceList.Count.Should().Be(replaceMore, "Replace more items by one key: " + String.Join(", ", replaceList));


            var replaceByIds = json.Where(x => !string.IsNullOrEmpty(x.ReplacedBy)).Select(x => x.ReplacedBy).Distinct().ToList();
            var notFound = replaceByIds.Where(x => !ids.ContainsKey(x)).ToList();
            notFound.Count.Should().Be(notFoundCount, string.Join(", ", notFound));

            json.Count(x => !string.IsNullOrEmpty(x.ReplacedBy)).Should().BeGreaterOrEqualTo(replaceByIds.Count);
        }

        private List<VmIndustrialClassJsonItem> FillChildren(List<VmIndustrialClassJsonItem> list)
        {
            //            Dictionary<string, VmFintoJsonItem> all = list.Select(x => new VmFintoJsonItem { Id = x.Code, Label = x.Name, Finnish = x.Name /* OntologyType = x.Level*/}).ToDictionary(x => x.Id);
            Dictionary<int, VmIndustrialClassJsonItem> parents = new Dictionary<int, VmIndustrialClassJsonItem>
            {
                { 1, null},
                { 2, null},
                { 3, null},
                { 4, null},
                { 5, null},
            };
            foreach (var item in list)
            {
                VmIndustrialClassJsonItem root = null;
                int parentLevel = item.Level - 1;
                parents[item.Level] = item;
                if (parentLevel > 0 && parentLevel < 5)
                {
                    root = parents[parentLevel];
                }
                if (root != null)
                {
                    if (root.Children == null)
                    {
                        root.Children = new List<VmIndustrialClassJsonItem>();
                    }
                    root.Children.Add(item);
//                    parent.Parents.Count.Should()
//                        .BeLessThan(2, $"{x} not in {parent.Parents.FirstOrDefault()}. Parent {item.Id}");
                    item.Parent = root.Code;
                }
            }
            return list.Where(x => string.IsNullOrEmpty(x.Parent)).ToList();
        }

//        [Fact]
        public void GetList()
        {
            Dictionary<string, List<VmJsonTypeItem>> types = new Dictionary<string, List<VmJsonTypeItem>>();
            var resourceLoader = new ResourceLoader();
            GetType<AccessRightEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.AccessRightType }, resourceLoader, types);
            GetType<AddressTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.AddressType }, resourceLoader, types);
            GetType<AppEnvironmentDataTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.AppEnvironmentDataType }, resourceLoader, types);
            GetType<AreaInformationTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.AreaInformationType }, resourceLoader, types);
            GetType<AreaTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.AreaType }, resourceLoader, types);
            GetType<AttachmentTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.AttachmentType }, resourceLoader, types);
            GetType<CoordinateTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.CoordinateType }, resourceLoader, types);
            GetType<DescriptionTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.DescriptionType }, resourceLoader, types);
            GetType<ExtraTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.ExtraType }, resourceLoader, types);
            GetType<NameTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.NameType }, resourceLoader, types);
            GetType<PhoneNumberTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.PhoneNumberType }, resourceLoader, types);
            GetType<PrintableFormChannelUrlTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.PrintableFormChannelUrlType }, resourceLoader, types);
            GetType<ProvisionTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.ProvisionType }, resourceLoader, types);
			GetType<AddressCharacterEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.AddressType }, resourceLoader, types);
            GetType<PublishingStatus>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.PublishingStatus }, resourceLoader, types);
            GetType<ServiceChannelConnectionTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.ServiceChannelConnectionType }, resourceLoader, types);
            GetType<ServiceChannelTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.ServiceChannelType }, resourceLoader, types);
            GetType<ServiceChargeTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.ServiceChargeType }, resourceLoader, types);
            GetType<ServiceFundingTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.ServiceFundingType }, resourceLoader, types);
            GetType<ServiceHoursTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.ServiceHours }, resourceLoader, types);
            GetType<ServiceTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.ServiceType }, resourceLoader, types);
            GetType<WebPageTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.WebPageType }, resourceLoader, types);

            File.WriteAllLines(@"d:\types.csv", types.SelectMany(type => type.Value.Select(v => new { Name = type.Key, Value = v }).Select(x => $"\"{x.Name}\";\"{x.Value.Code}\";\"{GetName(x.Value.Names, LanguageCode.fi)}\";\"{GetName(x.Value.Names, LanguageCode.en)}\"; \"{GetName(x.Value.Names, LanguageCode.sv)}\"; ")));
        }

        private string GetName(List<VmJsonTypeName> names, LanguageCode code)
        {
            string result = "";
            if (names != null)
            {
                result = names.Where(x => x.Language == code.ToString()).Select(x => x.Name).FirstOrDefault();
            }
            return result;
        }

        private List<VmJsonTypeItem> GetType<TEnumType>(IImportDefinition definition, ResourceLoader resourceLoader, IDictionary<string, List<VmJsonTypeItem>> dictionary) where TEnumType : struct
        {
            var list = resourceLoader.GetDeserializedResource<List<VmJsonTypeItem>>(definition);
            dictionary.Add(definition.ResourceName, list);
            return list;
        }

        private void GetUnique(List<VmFintoJsonItem> list, HashSet<string> uniqueItems)
        {
            foreach (var item in list)
            {
                if (!uniqueItems.Contains(item.Id))
                {
                    uniqueItems.Add(item.Id);
                }
                if (item.Narrower != null)
                {
                    GetUnique(item.Narrower, uniqueItems);
                }
            }
        }

    }
}
