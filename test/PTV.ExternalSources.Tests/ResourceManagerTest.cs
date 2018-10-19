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
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using Xunit;
using PTV.ExternalSources.Resources.Finto;
using PTV.ExternalSources.Resources.Types;
using PTV.Framework;

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
            HashSet<TypeItemImportDefinition.Resources> translated = new HashSet<TypeItemImportDefinition.Resources>
            {
                TypeItemImportDefinition.Resources.AreaInformationType,
                TypeItemImportDefinition.Resources.AreaType,
                TypeItemImportDefinition.Resources.ExtraType,
                TypeItemImportDefinition.Resources.ServiceChargeType,
                TypeItemImportDefinition.Resources.ServiceChannelConnectionType,
                TypeItemImportDefinition.Resources.PublishingStatus,
                TypeItemImportDefinition.Resources.ServiceType,
                TypeItemImportDefinition.Resources.GeneralDescriptionType,
                TypeItemImportDefinition.Resources.PhoneNumberType,
                TypeItemImportDefinition.Resources.ServiceFundingType,
                TypeItemImportDefinition.Resources.ServiceChannelType,
                TypeItemImportDefinition.Resources.ProvisionType,
                TypeItemImportDefinition.Resources.PrintableFormChannelUrlType,

            };
            Dictionary<TypeItemImportDefinition.Resources, List<VmJsonTypeItem>> types = new Dictionary<TypeItemImportDefinition.Resources, List<VmJsonTypeItem>>();
            var resourceLoader = new ResourceLoader();
            foreach (var resource in Enum.GetValues(typeof(TypeItemImportDefinition.Resources)).Cast<TypeItemImportDefinition.Resources>())
            {
                GetType(new TypeItemImportDefinition {Resource = resource}, resourceLoader, types);
            }

            File.WriteAllLines(@"d:\types.csv", types.SelectMany(type => type.Value.Select(v => new { Name = type.Key.ToString(), Value = v }).Select(x => $"\"{x.Name}\";\"{x.Value.Code}\";\"{translated.Contains(type.Key)}\";\"{GetName(x.Value.Names, "fi")}\";\"{GetName(x.Value.Names, "en")}\";\"{GetName(x.Value.Names, "sv")}\";")));
        }

        [Fact]
        public void GetJson()
        {
            const string OutputPath = @"d:\semi\final";
            
            if (!File.Exists(CsvOptions.FileName)) return;

            if (!Directory.Exists(OutputPath))
            {
                Directory.CreateDirectory(OutputPath);
            }

            var csv = File.ReadAllLines(CsvOptions.FileName, CsvOptions.Encoding)
                .Skip(CsvOptions.FirstLineIsHeader ? 1 : 0)
                .Select(csvLine => CsvImportItem.FromCsv(csvLine, CsvOptions.Delimiter))
                .ToList();

            var groupedByTypeName = csv.GroupBy(i => i.Table);
            
            var types = new Dictionary<TypeItemImportDefinition.Resources, List<VmJsonTypeItem>>();
            var resourceLoader = new ResourceLoader();
            foreach (var resource in Enum.GetValues(typeof(TypeItemImportDefinition.Resources)).Cast<TypeItemImportDefinition.Resources>())
            {
                GetType(new TypeItemImportDefinition {Resource = resource}, resourceLoader, types);
            }

            foreach (var type in types)
            {
                var csvType = groupedByTypeName.SingleOrDefault(x => x.Key == type.Key.ToString());
                if (csvType == null) continue;
                var isTranslated = false;
                
                foreach (var vmJsonTypeItem in type.Value)
                {
                    var csvItem = csvType.SingleOrDefault(a => a.Code == vmJsonTypeItem.Code);
                    if (csvItem == null) continue;
                    if (!csvItem.IsTranslated) continue;
                    isTranslated = true;
                    
                    // fi
                    if (!csvItem.Fi.IsNullOrEmpty())
                    {
                        var fi = vmJsonTypeItem.Names.SingleOrDefault(l => l.Language.ToLower() == "fi");
                        if (fi != null && fi.Name != csvItem.Fi) fi.Name = csvItem.Fi;
                        if (fi == null) vmJsonTypeItem.Names.Add(new VmJsonTypeName {Language = "fi", Name = csvItem.Fi});
                    }

                    // sv
                    if (!csvItem.Sv.IsNullOrEmpty())
                    {
                        var sv = vmJsonTypeItem.Names.SingleOrDefault(l => l.Language.ToLower() == "sv");
                        if (sv != null && sv.Name != csvItem.Sv) sv.Name = csvItem.Sv;
                        if (sv == null) vmJsonTypeItem.Names.Add(new VmJsonTypeName {Language = "sv", Name = csvItem.Sv});
                    }
                    
                    // En
                    if (!csvItem.En.IsNullOrEmpty())
                    {
                        var en = vmJsonTypeItem.Names.SingleOrDefault(l => l.Language.ToLower() == "en");
                        if (en != null && en.Name != csvItem.En) en.Name = csvItem.En;
                        if (en == null) vmJsonTypeItem.Names.Add(new VmJsonTypeName {Language = "en", Name = csvItem.En});
                    }
                    
                    // se
                    if (!csvItem.Se.IsNullOrEmpty())
                    {
                        var se = vmJsonTypeItem.Names.SingleOrDefault(l => l.Language.ToLower() == "se");
                        if (se != null && se.Name != csvItem.Se) se.Name = csvItem.Se;
                        if (se == null) vmJsonTypeItem.Names.Add(new VmJsonTypeName {Language = "se", Name = csvItem.Se});
                    }
                    
                    // smn
                    if (!csvItem.Smn.IsNullOrEmpty())
                    {
                        var smn = vmJsonTypeItem.Names.SingleOrDefault(l => l.Language.ToLower() == "smn");
                        if (smn != null && smn.Name != csvItem.Smn) smn.Name = csvItem.Smn;
                        if (smn == null) vmJsonTypeItem.Names.Add(new VmJsonTypeName {Language = "smn", Name = csvItem.Smn});
                    }
                    
                    // sms
                    if (!csvItem.Sms.IsNullOrEmpty())
                    {
                        var sms = vmJsonTypeItem.Names.SingleOrDefault(l => l.Language.ToLower() == "sms");
                        if (sms != null && sms.Name != csvItem.Smn) sms.Name = csvItem.Sms;
                        if (sms == null) vmJsonTypeItem.Names.Add(new VmJsonTypeName {Language = "sms", Name = csvItem.Sms});
                    }
                }

                if (isTranslated)
                {
                    var fileName = $"{type.Key}.json";
                    var filePath = Path.Combine(OutputPath, fileName);
                
                    var json = JsonConvert.SerializeObject(type.Value, Formatting.Indented);
                    File.WriteAllText(filePath, json, CsvOptions.Encoding);    
                }
            }
        }

        private string GetName(List<VmJsonTypeName> names, string code)
        {
            string result = "";
            if (names != null)
            {
                result = names.Where(x => x.Language == code).Select(x => x.Name).FirstOrDefault();
            }
            return result;
        }

        private List<VmJsonTypeItem> GetType(TypeItemImportDefinition definition, ResourceLoader resourceLoader, IDictionary<TypeItemImportDefinition.Resources, List<VmJsonTypeItem>> dictionary)
        {
            var list = resourceLoader.GetDeserializedResource<List<VmJsonTypeItem>>(definition);
            dictionary.Add(definition.Resource, list);
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

        private struct CsvOptions
        {
//            internal const string FileName = @"d:\semi\types.csv";
            internal const string FileName = @"d:\semi\csv - utf8.csv";
            internal static readonly char[] Delimiter = {';'};
            internal static Encoding  Encoding => Encoding.UTF8;
            internal static bool FirstLineIsHeader => true;
        }

        private class CsvImportItem
        {
            public string Table { get; set; }
            public string Code { get; set; }
            public bool IsTranslated { get; set; }
            public string Fi { get; set; }
            public string En { get; set; }
            public string Sv { get; set; }
            public string Se { get; set; }
            public string Sms { get; set; }
            public string Smn { get; set; }

            public static CsvImportItem FromCsv(string line, char[] delimiter)
            {
                var values = line.Split(delimiter);
                return new CsvImportItem
                {
                    Table = values[0],
                    Code = values[1],
                    IsTranslated = Convert.ToBoolean(values[2]),
                    Fi = values[3],
                    En = values[4],
                    Sv = values[5],
                    Se = values[6],
                    Sms = values[7],
                    Smn = values[8]
                };
            }
        }
    }
}
