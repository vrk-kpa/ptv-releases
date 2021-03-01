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
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PTV.Domain.Model.Models.Converters.QualityAgent
{
    /// <summary>
    ///
    /// </summary>
    public class QualityAgentFieldPathConverter : JsonConverter
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var p = JsonConvert.SerializeObject(value);

            //writer.WriteRaw(p);
            writer.WriteRawValue(p);
        }

        private string GetValidPath(string path)
        {
            return path
                .Replace("type = ", string.Empty)
                .Replace("language = ", string.Empty);
        }

        private string GetReducedPath(string path, string subStringKey)
        {
            if (path.Contains(subStringKey) && path.Length > subStringKey.Length)
            {
                var startPos = path.IndexOf(subStringKey, StringComparison.CurrentCultureIgnoreCase);
                path = path.Substring(startPos + subStringKey.Length);
            }

            return path;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            const string pathValidKey = "_key = ";

            if (reader.TokenType == JsonToken.StartArray)
            {
                var arrayToken = JArray.Load(reader);
                var list = arrayToken.ToObject<List<string>>();

                var pathWithoutKeyList = string.Join(".", list.Where(x => !x.Contains(pathValidKey)).Select(GetValidPath));
                var pathOnlyKeyList = string.Join(".", list.Where(x => x.Contains(pathValidKey)).Select(y => GetReducedPath(y, pathValidKey)));
                var result = !string.IsNullOrEmpty(pathOnlyKeyList)
                    ? $"{pathOnlyKeyList};{pathWithoutKeyList}"
                    : pathWithoutKeyList;

                return result.Split(".+.").ToList();
            }
            return null;

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

    }
}
