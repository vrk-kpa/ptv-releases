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

using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Domain.Model.Tests.Models.OpenApi
{
    public class ModelsTestBase
    {
        protected IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        protected void CheckProperties(IVmEntityBase model, int? version = null)
        {
            // Have to use JsonConvert to take JsonIgnore attribute into account!
            var modelProperties = JObject.Parse(JsonConvert.SerializeObject(model, Formatting.Indented)).ToIgnoreCaseDictionary();
            CheckProperties(modelProperties, version, model.GetType().Name);
        }

        protected void CheckProperties(IVmOpenApiBase model, int? version = null)
        {
            // Have to use JsonConvert to take JsonIgnore attribute into account!
            var modelProperties = JObject.Parse(JsonConvert.SerializeObject(model, Formatting.Indented)).ToIgnoreCaseDictionary();
            CheckProperties(modelProperties, version, model.GetType().Name);
        }

        protected void CheckListItemProperties(IVmOpenApiListItem model, int? version = null)
        {
            // Have to use JsonConvert to take JsonIgnore attribute into account!
            var modelProperties = JObject.Parse(JsonConvert.SerializeObject(model, Formatting.Indented)).ToIgnoreCaseDictionary();
            CheckProperties(modelProperties, version, model.GetType().Name);
        }

        protected void CheckServiceHourProperties<TModelOpeningTime>(IVmOpenApiServiceHourBase<TModelOpeningTime> serviceHours, int version)
            where TModelOpeningTime : IVmOpenApiDailyOpeningTime
        {
            // Have to use JsonConvert to take JsonIgnore attribute into account!
            var modelProperties = JObject.Parse(JsonConvert.SerializeObject(serviceHours, Formatting.Indented)).ToIgnoreCaseDictionary();
            CheckProperties(modelProperties, version, serviceHours.GetType().Name);
            var openingTime = serviceHours.OpeningHour.First();
            var openingTimeProperties = JObject.Parse(JsonConvert.SerializeObject(openingTime, Formatting.Indented)).ToIgnoreCaseDictionary();
            if (version == 11)
            {
                CheckProperties(openingTimeProperties, 8, openingTime.GetType().Name);
            }
            else
            {
                CheckProperties(openingTimeProperties, version, openingTime.GetType().Name);
            }
        }

        protected void CheckServiceServiceChannel(IVmOpenApiServiceServiceChannelBase relation, int? version = null)
        {
            // Have to use JsonConvert to take JsonIgnore attribute into account!
            var modelProperties = JObject.Parse(JsonConvert.SerializeObject(relation, Formatting.Indented)).ToIgnoreCaseDictionary();
            CheckProperties(modelProperties, version, relation.GetType().Name);
        }

        protected void CheckServiceServiceCollection(VmOpenApiServiceServiceCollection relation, int? version = null)
        {
            // Have to use JsonConvert to take JsonIgnore attribute into account!
            var modelProperties = JObject.Parse(JsonConvert.SerializeObject(relation, Formatting.Indented)).ToIgnoreCaseDictionary();
            CheckProperties(modelProperties, version, relation.GetType().Name);
        }

        protected void CheckProperties(Dictionary<string, object> modelProperties, int? version, string resourceName)
        {
            string fileName = "PTV.Domain.Model.Tests.Models.OpenApi.V" + version + ".Resources." + resourceName + ".json";
            if (version == null)
            {
                fileName = "PTV.Domain.Model.Tests.Models.OpenApi.Resources." + resourceName + ".json";
            }
            var validModel = GetJsonResource(fileName);
            // Check that all the properties can be found from the valid model
            foreach (var property in modelProperties)
            {
                var propertyName = property.Key;
                validModel.ContainsKey(propertyName).Should().BeTrue();
            }

            // Check that the model includes all the properties that exist in valid model
            foreach (var property in validModel)
            {
                var propertyName = property.Key;
                modelProperties.ContainsKey(propertyName).Should().BeTrue();
            }
        }

        protected void CheckOrganizationServices(IVmOpenApiOrganizationServiceBase items, int version)
        {
            // Have to use JsonConvert to take JsonIgnore attribute into account!
            var modelProperties = JObject.Parse(JsonConvert.SerializeObject(items, Formatting.Indented)).ToIgnoreCaseDictionary();
            CheckProperties(modelProperties, version, items.GetType().Name);
        }

        private Dictionary<string, object> GetJsonResource(string fileName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException(fileName);
                }
                var reader = new StreamReader(stream);
                return JObject.Parse(reader.ReadToEnd()).ToIgnoreCaseDictionary();
            }
        }


    }

    public static class JsonExtensions
    {
        public static Dictionary<string, object> ToIgnoreCaseDictionary(this JObject json)
        {
            return new Dictionary<string, object>(json.ToObject<Dictionary<string, object>>(), StringComparer.OrdinalIgnoreCase);
        }
    }
}
