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

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PTV.DataImport.Console
{
    internal class JsonHelper
    {
        private static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            MissingMemberHandling = MissingMemberHandling.Error
        };

        /// <summary>
        /// Desrialize JSON to an object.
        /// </summary>
        /// <typeparam name="T">type to deserialize</typeparam>
        /// <param name="json">JSON string</param>
        /// <returns>deserialized object</returns>
        /// <exception cref="Newtonsoft.Json.JsonSerializationException">If member is missing from the object to serialize to.</exception>
        internal static T Deserialize<T>(string json) where T: class
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(json, JsonHelper.DefaultJsonSerializerSettings);
        }
    }
}
