/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PTV.Framework;

namespace PTV.ExternalSources
{
    public enum ImportFileType
    {
        Csv,
        Json
    }
    public interface IImportDefinition
    {
        ImportFileType FileType { get; }
        string ResourceName { get; }
        string ResourcePath { get; }
    }

    [RegisterService(typeof(ResourceManager), RegisterType.Transient)]
    public class ResourceManager
    {
        public IList<string[]> GetCsvResource(CvsResources resourceName)
        {
            var resourceFile = resourceName+".csv";
            return GetCsvResource(resourceFile);
        }

        public IList<string[]> GetCsvResource(string resourceFile)
        {
            return LoadFile(resourceFile, reader =>
            {
                var result = new List<string[]>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    result.Add(values);
                }
                return result;
            });
        }

        public string GetJsonResource(JsonResources resourceName)
        {
            return LoadFile(resourceName + ".json", reader => reader.ReadToEnd());
        }

        public T GetDesrializedJsonResource<T>(JsonResources resourceName)
        {
            return JsonConvert.DeserializeObject<T>(GetJsonResource(resourceName));
        }

        private T LoadFile<T>(string resourceName, Func<StreamReader, T> loadDataFunc)
        {
            var fileName = "PTV.ExternalSources.Resources." + resourceName;
            using (var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream(fileName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("File not found.", fileName);
                }
                var reader = new StreamReader(stream);

                return loadDataFunc(reader);
            }
        }

    }

    [RegisterService(typeof(ResourceLoader), RegisterType.Transient)]
    public class ResourceLoader
    {
        private string BuildPath(params string[] resourcePath)
        {
            return resourcePath.Aggregate((aggregated, path) => $"{aggregated}.{path}");
        }
        public string GetResource(IImportDefinition resourceDefinition)
        {
            return LoadFile(BuildPath(resourceDefinition.ResourcePath, resourceDefinition.ResourceName, resourceDefinition.FileType.ToString().ToLower()), reader => reader.ReadToEnd());
        }

        public T GetDeserializedResource<T>(IImportDefinition resourceDefinition)
        {
            return JsonConvert.DeserializeObject<T>(GetResource(resourceDefinition));
        }

        private T LoadFile<T>(string resourceName, Func<StreamReader, T> loadDataFunc)
        {
            var fileName = "PTV.ExternalSources.Resources." + resourceName;
            using (var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream(fileName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"File {fileName} not found.");
                }
                var reader = new StreamReader(stream);

                return loadDataFunc(reader);
            }
        }


        public IList<string[]> GetCsvResource(string resourceFile)
        {
            return LoadFile(resourceFile, reader =>
            {
                var result = new List<string[]>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    result.Add(values);
                }
                return result;
            });
        }

    }

    public enum CvsResources
    {
        CountryCode,
    }

    public enum JsonResources
    {
        LifeSituations,
        ServiceClasses,
        TargetGroups,
        Ontologies,
        MunicipalityOrganizations,
        Municipality,
        LanguageCodes,
        PostalCode,
        CountryCodes,
        Province,
        BusinessRegion,
        HospitalRegion,
        DigitalAuthorizations,
        InvalidAreas,
        AstiTypes,
        UserAccessRightsGroup,
        OrganizationTypeRestrictions,
        TasksConfiguration,
        TempSahaMapping
    }
}
