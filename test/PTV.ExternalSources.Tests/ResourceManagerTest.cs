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
using PTV.Domain.Model.Models;
using Xunit;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PTV.Domain.Model.Models.Import;
using PTV.ExternalSources.Resources.Finto;
using PTV.ExternalSources.Resources.FintoFi;
using PTV.Framework;
using CoreExtensions = PTV.Framework.CoreExtensions;

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
//        [InlineData(FintoItemImportDefinition.Resources.LifeSituations, 14, 19)]
//        [InlineData(FintoItemImportDefinition.Resources.TargetGroups, 3, 18)]
//        [InlineData(FintoItemImportDefinition.Resources.ServiceClasses, 27, 221)]
        [InlineData(FintoItemImportDefinition.Resources.OntologyAll, 35301, 35301)]
//        [InlineData(FintoItemImportDefinition.Resources.juho, 5, 152387)]
//        [InlineData(FintoItemImportDefinition.Resources.jupo, 23, 153606)]
//        [InlineData(FintoItemImportDefinition.Resources.liito, 3, 155818)]
//        [InlineData(FintoItemImportDefinition.Resources.tero, 73, 28840)]
//        [InlineData(FintoItemImportDefinition.Resources.tsr, 1790, 155779)]
//        //[InlineData(JsonResources.Ontologies)]
////        [InlineData(JsonResources.Municipality, 297)]
////        [InlineData(JsonResources.Organizations, 10, typeof(List<VmJsonOrganization>))]
        public void JsonLoadDeserializedTest(FintoItemImportDefinition.Resources resource, int expectedCountMain, int expectedCountAll)
        {
            var resourceManager = new ResourceLoader();
//            MethodInfo method = resourceManager.GetType().GetMethod("GetDesrializedJsonResource");
//            MethodInfo generic = method.MakeGenericMethod(customNonFintoType);
//            IList json = generic.Invoke(resourceManager, new object[] { resource }) as IList;
            var json = resourceManager.GetDesrializedResource<List<VmFintoJsonItem>>(new FintoItemImportDefinition { Resource = resource});
            Assert.NotNull(json);
            json.Count.Should().Be(expectedCountMain);
            FillChildren(json);

            CountData(json).Should().Be(expectedCountAll);

//            File.WriteAllText($@"D:\{resource}_update.json", JsonConvert.SerializeObject(json));
        }

        private int CountData(List<VmFintoJsonItem> list)
        {
            int count = list.Count + list.Sum(x => CountData(x.Narrower ?? new List<VmFintoJsonItem>()));
            return count;
        }

        private void FillChildren(List<VmFintoJsonItem> list)
        {
            Dictionary<string, VmFintoJsonItem> all = list.ToDictionary(x => x.Id);
            foreach (var item in list)
            {
                item.Children?.ForEach(x =>
                {
                    VmFintoJsonItem child = all.TryGet(x);
//                    child.Should().NotBeNull($"{x} not in {list.Count}. Parent {item.Id}");
                    if (child != null)
                    {


                        if (item.Narrower == null)
                        {
                            item.Narrower = new List<VmFintoJsonItem>();
                        }
                        if (child.Parents == null)
                        {
                            child.Parents = new List<string>();
                        }
                        item.Narrower.Add(child);
                        child.Parents.Count.Should().BeLessThan(2, $"{x} not in {child.Parents.FirstOrDefault()}. Parent {item.Id}");
                        child.Parents.Add(item.Id);
                    }
                });
            }

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

//        private void AddChild(VmIndustrialClassJsonItem parent, )
//
//        [Theory]
//        [InlineData(FintoItemImportDefinition.Resources.yso, 5, 41323)]
//        [InlineData(FintoItemImportDefinition.Resources.jupo, 23, 41582)]
//        [InlineData(FintoItemImportDefinition.Resources.juho, 5, 41323)]
//        [InlineData(FintoItemImportDefinition.Resources.liito, 3, 42952)]
//        [InlineData(FintoItemImportDefinition.Resources.tero, 73, 27442)]
//        [InlineData(FintoItemImportDefinition.Resources.tsr, 1790, 43224)]
//        //[InlineData(JsonResources.Ontologies)]
//        //        [InlineData(JsonResources.Municipality, 297)]
//        //        [InlineData(JsonResources.Organizations, 10, typeof(List<VmJsonOrganization>))]
//        public void JsonLoadDeserializedFilteredTest(FintoItemImportDefinition.Resources resource, int expectedCountMain, int expectedCountAll)
//        {
//            HashSet<string> items = new HashSet<string>();
//            var resourceManager = new ResourceLoader();
//            //            MethodInfo method = resourceManager.GetType().GetMethod("GetDesrializedJsonResource");
//            //            MethodInfo generic = method.MakeGenericMethod(customNonFintoType);
//            //            IList json = generic.Invoke(resourceManager, new object[] { resource }) as IList;
//            var json = resourceManager.GetDesrializedResource<List<VmFintoJsonItem>>(new FintoItemImportDefinition { Resource = resource });
//            Assert.NotNull(json);
//            json.Count.Should().Be(expectedCountMain);
//            GetUnique(json, items);
//            //string ids = string.Join(", ", items);
//            //File.WriteAllText($@"D:\duplicates {resource}", ids);
//            items.Count.Should().Be(expectedCountAll);
//        }

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

        private void CheckFintoItem(FintoFiJsonItem item)
        {
            item.Type.Should().NotBeNullOrEmpty();
//            if (item.Type == "skos:Concept")
//            {
//                Console.WriteLine(item.Notation);
//                item.Notation.Should().NotBeNullOrEmpty();
//            }
            item.Labels.Count.Should().BeGreaterThan(0);
        }

        private FintoFiJsonItem CreateItem(FintoFiHierarchy hierarchyItem, string parentUri = "")
        {
            var item = new FintoFiJsonItem
            {
                Uri = hierarchyItem.Uri,
                Broader = string.IsNullOrEmpty(parentUri) ? null : new FintoFiJsonParent { Uri = parentUri },
                Type = "Onto",
            };
            item.Labels.Add(new FintoFiJsonLabel { Lang = "fi", Value = hierarchyItem.PrefLabel });
            return item;
        }

        private FintoFiJsonItem CreateItem(FintoFiNarrower narrower, string parentUri)
        {
            var item = new FintoFiJsonItem
            {
                Uri = narrower.Uri,
                Broader = string.IsNullOrEmpty(parentUri) ? null : new FintoFiJsonParent { Uri = parentUri },
                Type = "Onto",
            };
            item.Labels.Add(new FintoFiJsonLabel { Lang = "fi", Value = narrower.Label + narrower.PrefLabel });
            return item;
        }



        [Theory]
        [InlineData(CvsResources.CountryCode)]
        public void CsvLoadTest(CvsResources resource)
        {
            var resourceManager = new ResourceManager();
            var json = resourceManager.GetCsvResource(resource);
            Assert.NotNull(json);
        }
    }
}
