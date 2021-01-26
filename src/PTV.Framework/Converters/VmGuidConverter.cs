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

namespace PTV.Framework.Converters
{
    public abstract class VmGuidConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var p = JsonConvert.SerializeObject(value);
            //writer.WriteRaw(p);
            writer.WriteRawValue(p);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                Guid? id = (reader.Value as string).ParseToGuid();
                return GetObject(id);
            }
            return existingValue;

        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        protected abstract object GetObject(Guid? id);
    }

    public class NotNullGuidConverter : VmGuidConverter
    {
        protected override object GetObject(Guid? id)
        {
            return id ?? Guid.Empty;
        }
    }

    public class IntEnumConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var p = JsonConvert.SerializeObject(value);
            //writer.WriteRaw(p);
            writer.WriteRawValue(p);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                return Convert.ChangeType(reader.Value, objectType);
            }
            if (reader.TokenType == JsonToken.StartArray)
            {
                return (int)reader.Value;
            }
            return default(int);

        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

    }

    public abstract class VmGuidListConverter<T> : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var p = JsonConvert.SerializeObject(value);
            writer.WriteRawValue(p);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var ids = serializer.Deserialize<List<string>>(reader);
                var result = ids.Select(x => GetObject(x.ParseToGuid())).ToList();
                return result;
            }
            return existingValue;

        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsEnumerable();
        }

        public abstract T GetObject(Guid? id);
    }
}
