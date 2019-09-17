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
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using System.Collections.Generic;
using PTV.Framework;

namespace PTV.Localization.Services.Model
{
    public class TranslationOptions : Dictionary<string, object>, ITranslationOptions
    {
        public TranslationOptions()
        {
        }

        public TranslationOptions(IDictionary<string, object> dictionary) : base(dictionary)
        {
        }

        public TranslationOptions(IEnumerable<KeyValuePair<string, object>> collection) : base(collection)
        {
        }

        public void AddRange(IEnumerable<KeyValuePair<string, object>> collection)
        {
            collection?.ForEach(x =>
            {
                var (key, value) = x;
                this[key] = value;
            });
        }

        public string GetValue(string key, string defaultValue = null)
        {
            if (this.TryGet(key) is string value)
            {
                return value;
            }

            return defaultValue;
        }

        public IEnumerable<string> GetValues(string key)
        {
            if (this.TryGet(key) is IEnumerable<string> value)
            {
                return value;
            }

            return new List<string>();
        }
    }
}