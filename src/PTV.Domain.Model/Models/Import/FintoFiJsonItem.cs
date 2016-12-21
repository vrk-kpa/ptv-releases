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
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PTV.Domain.Model.Models.Import
{
    public class FintoFiJsonItem
    {
        public FintoFiJsonItem()
        {
            _additionalData = new Dictionary<string, JToken>();
            Labels = new List<FintoFiJsonLabel>();
        }

        public string Uri { get; set; }
        public string Type { get; set; }
        public FintoFiJsonParent Broader { get; set; }

        public string Notation { get; set; }

        public FintoFiJsonLabel Label => Labels.FirstOrDefault();

        public List<FintoFiJsonLabel> Labels { get; set; }

        [JsonExtensionData] private IDictionary<string, JToken> _additionalData;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            JToken notation;
            if (_additionalData.TryGetValue("skos:notation", out notation))
            {
                Notation = notation.ToString();
            }

            JToken labels;
            if (_additionalData.TryGetValue("prefLabel", out labels))
            {
                var tokentType = labels.Type;
                if (tokentType == JTokenType.Array)
                {
                    var languages = labels.Children().ToList();
                    foreach (var language in languages)
                    {
                        Labels.Add(JsonConvert.DeserializeObject<FintoFiJsonLabel>(language.ToString()));
                    }
                }
                else
                {
                    Labels.Add(JsonConvert.DeserializeObject<FintoFiJsonLabel>(labels.ToString()));
                }

            }
        }
    }

//    public class FintoFiJsonOntoItem
//    {
//        public FintoFiJsonOntoItem()
//        {
//            _additionalData = new Dictionary<string, JToken>();
//            Labels = new List<FintoFiJsonLabel>();
//        }
//
//        public string Uri { get; set; }
//        public string Type { get; set; }
//        public FintoFiJsonParent Broader { get; set; }
//
//        public string Notation { get; set; }
//
//        public List<FintoFiJsonLabel> Labels { get; set; }
//
//        [JsonExtensionData]
//        private IDictionary<string, JToken> _additionalData;
//
//        [OnDeserialized]
//        private void OnDeserialized(StreamingContext context)
//        {
//            // SAMAccountName is not deserialized to any property
//            // and so it is added to the extension data dictionary
//            //            throw new Exception($"{_additionalData.Keys.Count} - {string.Join(", ",_additionalData.Keys)}");
//            JToken notation;
//            if (_additionalData.TryGetValue("skos:notation", out notation))
//            {
//                Notation = notation.ToString();
//            }
//
//            JToken labels;
//            if (_additionalData.TryGetValue("prefLabel", out labels))
//            {
//                var tokentType = labels.Type;
//                if (tokentType == JTokenType.Array)
//                {
//                    var languages = labels.Children().ToList();
//                    foreach (var language in languages)
//                    {
//                        Labels.Add(JsonConvert.DeserializeObject<FintoFiJsonLabel>(language.ToString()));
//                    }
//                }
//                else
//                {
//                    Labels.Add(JsonConvert.DeserializeObject<FintoFiJsonLabel>(labels.ToString()));
//                }
//
//            }
//        }
//    }

    public class FintoFiJsonLabel
    {
        public string Lang { get; set; }
        public string Value { get; set; }
    }

    public class FintoFiJsonParent
    {
        public string Uri { get; set; }
    }

//    public class FintoJsonData
//    {
//        public List<FintoFiJsonOntoItem> Graph { get; set; }
//    }


    public class FintoFiHierarchy
    {
        public string Uri { get; set; }

        public string Top { get; set; }

        public List<FintoFiNarrower> Narrower { get; set; }

        public string PrefLabel { get; set; }

    }

    public class FintoFiNarrower
    {
        public string Uri { get; set; }

        public string Label { get; set; }
        public string PrefLabel { get; set; }

        public bool HasChildren { get; set; }

    }

}
