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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PTV.Domain.Model.Models.QualityAgent.Output;
using PTV.Framework;

namespace PTV.Domain.Model.Models.Converters.QualityAgent
{
    /// <summary>
    /// 
    /// </summary>
    public class QualityAgentProcessedConverter : JsonConverter
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
            if (reader.TokenType == JsonToken.StartObject)
            {
                var x = JObject.Load(reader);
                var dataToken = x.TryGet("data");
                if (dataToken == null || dataToken.Type == JTokenType.String || dataToken.Type == JTokenType.Null)
                {
                    var result = x.ToObject<VmQualityAgentProcessed>();
//                        serializer.Populate(reader, result);
//                    result.Data = dataToken?.Value<string>();
                    result.Path = result.Path?.Select(GetValidPath).ToList();
                    return result;
                }
                else if (dataToken.Type == JTokenType.Object)
                {
                    var result = new VmQualityAgentProcessed();
                    var compare = dataToken.ToObject<VmQualityAgentCompareProcessed>();
                    result.Data = string.Join(";", new [] { compare.Compare1?.Data, compare.Compare2.Data}.WhereNotNull() );
                    result.Path = compare.Compare1 != null ? new List<string>(compare.Compare1?.Path.Select(GetValidPath)) : new List<string>();
                    result.Path.AddRange(compare.Compare2.Path.Select(GetValidPath));
                    return result;
                }


//                Console.WriteLine(x.SerializeObject());
                return null;
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
