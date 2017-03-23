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

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using FluentAssertions;
using NuGet.Packaging;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using Xunit;
using PTV.ExternalSources.Resources.Finto;
using PTV.ExternalSources.Resources.Types;

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
        [InlineData(JsonResources.Organizations)]
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
            var json = resourceManager.GetDesrializedResource<List<VmIndustrialClassJsonItem>>(new FintoItemImportDefinition { Resource = FintoItemImportDefinition.Resources.IndustrialClasses });
            Assert.NotNull(json);
            json.Count.Should().Be(1805);
            var finto = FillChildren(json);

            //CountData(finto).Should().Be(expectedCountAll);

            //File.WriteAllText($@"D:\IndustrialClassesTree.json", JsonConvert.SerializeObject(finto));
        }

        [Theory]
        [InlineData(FintoItemImportDefinition.Resources.LifeSituations, 14, 19)]
        [InlineData(FintoItemImportDefinition.Resources.TargetGroups, 3, 17)]
        [InlineData(FintoItemImportDefinition.Resources.ServiceClasses, 27, 212)]
        [InlineData(FintoItemImportDefinition.Resources.OntologyAll, 2, 35775)]
        public void JsonLoadDeserializedTest(FintoItemImportDefinition.Resources resource, int expectedCountMain, int expectedCountAll)
        {
            var resourceManager = new ResourceLoader();
//            MethodInfo method = resourceManager.GetType().GetMethod("GetDesrializedJsonResource");
//            MethodInfo generic = method.MakeGenericMethod(customNonFintoType);
//            IList json = generic.Invoke(resourceManager, new object[] { resource }) as IList;
            var json = resourceManager.GetDesrializedResource<List<VmServiceViewsJsonItem>>(new FintoItemImportDefinition { Resource = resource});
            Assert.NotNull(json);
            json.Count(x => x.BroaderURIs == null || x.BroaderURIs.Count == 0).Should().Be(expectedCountMain);
//            FillChildren(json);

            json.Count.Should().Be(expectedCountAll);

//            File.WriteAllText($@"D:\{resource}_update.json", JsonConvert.SerializeObject(json));
        }

//        private void FillChildren(List<VmFintoJsonItem> list)
//        {
//            Dictionary<string, VmFintoJsonItem> all = list.ToDictionary(x => x.Id);
//            foreach (var item in list)
//            {
//                item.Children?.ForEach(x =>
//                {
//                    VmFintoJsonItem child = all.TryGet(x);
////                    child.Should().NotBeNull($"{x} not in {list.Count}. Parent {item.Id}");
//                    if (child != null)
//                    {
//
//
//                        if (item.Narrower == null)
//                        {
//                            item.Narrower = new List<VmFintoJsonItem>();
//                        }
//                        if (child.Parents == null)
//                        {
//                            child.Parents = new List<string>();
//                        }
//                        item.Narrower.Add(child);
//                        child.Parents.Count.Should().BeLessThan(2, $"{x} not in {child.Parents.FirstOrDefault()}. Parent {item.Id}");
//                        child.Parents.Add(item.Id);
//                    }
//                });
//            }
//        }

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

       // [Fact]
        public void GetList()
        {
            Dictionary<string, List<VmJsonTypeItem>> types = new Dictionary<string, List<VmJsonTypeItem>>();
            var resourceLoader = new ResourceLoader();
            GetType<NameTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.NameType }, resourceLoader, types);
            GetType<PhoneNumberTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.PhoneNumberType }, resourceLoader, types);
            GetType<DescriptionTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.DescriptionType }, resourceLoader, types);
            GetType< AddressTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.AddressType }, resourceLoader, types);
            GetType<WebPageTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.WebPageType }, resourceLoader, types);
            GetType<ServiceCoverageTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.ServiceCoverageType }, resourceLoader, types);
            GetType<PublishingStatus>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.PublishingStatus }, resourceLoader, types);
            GetType< RoleTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.RoleType }, resourceLoader, types);
            GetType< OrganizationTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.OrganizationType }, resourceLoader, types);
            GetType< ServiceChannelTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.ServiceChannelType }, resourceLoader, types);
            GetType< ProvisionTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.ProvisionType }, resourceLoader, types);
            GetType< ServiceChargeTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.ServiceChargeType }, resourceLoader, types);
            GetType< ServiceHoursTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.ServiceHours }, resourceLoader, types);
//            SeedType<ExceptionHoursStatus>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.ExceptionHoursStatus });
            GetType< AttachmentTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.AttachmentType }, resourceLoader, types);
            GetType< PrintableFormChannelUrlTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.PrintableFormChannelUrlType }, resourceLoader, types);
            GetType< ServiceTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.ServiceType }, resourceLoader, types);

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
            var list = resourceLoader.GetDesrializedResource<List<VmJsonTypeItem>>(definition);
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

//        [Theory]
//        [InlineData(FintoFiItemImportDefinition.Resources.TargetGroups, 18)]
//        [InlineData(FintoFiItemImportDefinition.Resources.ServiceClasses, 221)]
//        public void JsonImportDefinitionDeserializedTest(FintoFiItemImportDefinition.Resources resource, int expectedCount)
//        {
//            var resourceManager = new ResourceLoader();
//            var importDefinition = new FintoFiItemImportDefinition();
//            importDefinition.Resource = resource;
//            //MethodInfo method = resourceManager.GetType().GetMethod("GetDesrializedJsonResource");
//            //MethodInfo generic = method.MakeGenericMethod(customNonFintoType);
//            //IList json = generic.Invoke(resourceManager, new object[] { resource }) as IList;
//            var json = resourceManager.GetDesrializedResource<List<FintoFiJsonItem>>(importDefinition);
//            Assert.NotNull(json);
//            json.Count.Should().Be(expectedCount);
////            foreach (var item in json)\
////            {
////                CheckFintoItem(item);
////            }
//        }


//        [Theory]
//        [InlineData(CvsResources.CountryCode)]
//        public void CsvLoadTest(CvsResources resource)
//        {
//            var resourceManager = new ResourceManager();
//            var json = resourceManager.GetCsvResource(resource);
//            Assert.NotNull(json);
//        }


    }

//
}
