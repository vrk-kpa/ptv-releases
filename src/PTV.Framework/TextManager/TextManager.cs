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
using Microsoft.Extensions.Logging;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using PTV.Framework.TextManager.Models;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace PTV.Framework.TextManager
{
    /// <summary>
    /// Converting json to markdown and back
    /// </summary>
    [RegisterService(typeof(ITextManager), RegisterType.Transient)]
    public class TextManager : ITextManager
    {
        private readonly ILogger logger;
        private readonly string LineBreak = "\n";
        private Random random = new Random();

        public TextManager(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<TextManager>();
        }

        public string ConvertMarkdownToJson(string text)
        {
            string result = string.Empty;
            var defaultJsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                MissingMemberHandling = MissingMemberHandling.Error
            };

            try
            {
                result = JsonConvert.SerializeObject(ParseMarkdownToTextBlock(text), defaultJsonSerializerSettings);
            }
            catch (JsonSerializationException ex)
            {
                throw new Exception($"Serialize object failed on markdown text: {text}. {ex.Message}", ex);
            }

            return result;
        }

        public string ConvertTextWithLineBreaksToJson(string text)
        {
            string result = string.Empty;
            var defaultJsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                MissingMemberHandling = MissingMemberHandling.Error
            };

            try
            {
                result = JsonConvert.SerializeObject(ParseLineBreaksToTextBlock(text), defaultJsonSerializerSettings);
            }
            catch (JsonSerializationException ex)
            {
                throw new Exception($"Serialize object failed on markdown text: {text}. {ex.Message}", ex);
            }

            return result;
        }

        public string ConvertToPureText(string json)
        {
            var dataToConvert = DeserializeJson(json);
            return string.Join(" ", dataToConvert?.Blocks.Where(x => !string.IsNullOrWhiteSpace(x.Text)).Select(x => x.Text));
        }

        public string ConvertToMarkDown(string json)
        {
            int counter = 0;
            var dataToConvert = DeserializeJson(json);
            var lastBlock = dataToConvert.Blocks.Count > 0 ? dataToConvert.Blocks.Last() : null;
            StringBuilder result = new StringBuilder();

            foreach (var block in dataToConvert.Blocks)
            {
                if (TextFormatClientTypeEnum.orderedListItem == block.Type)
                {
                    result.AppendFormat(ParseToMarkdownFormat(block.Type), ++counter, block.Text);
                }
                else
                {
                    result.AppendFormat(ParseToMarkdownFormat(block.Type), block.Text);
                    counter = 0;
                }

                result.AppendFormat("{0}", block != lastBlock ? LineBreak : string.Empty);
            }

            return result.ToString();
        }

        private VmTextBlock ParseMarkdownToTextBlock(string text)
        {
            VmTextBlock textBlock = new VmTextBlock();
            var lines = text?.Split('\n') ?? new string[0] { };

            var typePatterns = new Dictionary<TextFormatClientTypeEnum, string>()
            {
                {TextFormatClientTypeEnum.unorderedListItem, @"\*"}, //ordered
                {TextFormatClientTypeEnum.orderedListItem, @"\d+\."}, //unoredered
                {TextFormatClientTypeEnum.unstyled, @"[^\d+\.\*\s]+"} //rest
            };

            foreach (var line in lines)
            {
                int index = int.MaxValue;
                int textLength = 0;
                var lineText = line;
                TextFormatClientTypeEnum type = TextFormatClientTypeEnum.unstyled;

                foreach (KeyValuePair<TextFormatClientTypeEnum, string> valueTypePattern in typePatterns)
                {
                    if (Regex.IsMatch(lineText, valueTypePattern.Value)) //find first occurence
                    {
                        var match = Regex.Match(lineText, valueTypePattern.Value);
                        if (match.Index < index)
                        {
                            index = match.Index;
                            textLength = match.Length;
                            type = valueTypePattern.Key;
                        }
                    }
                }

                if (type == TextFormatClientTypeEnum.unorderedListItem)
                {
                    lineText = line.Remove(index, 1);
                }

                if (type == TextFormatClientTypeEnum.orderedListItem)
                {
                    lineText = line.Remove(index, textLength);
                }

                textBlock.Blocks.Add(new VmTextLine() { Text = lineText, Type = type, Key = GetUniqueKey(5) });
            }

            return textBlock;
        }

        private VmTextBlock ParseLineBreaksToTextBlock(string text)
        {
            VmTextBlock textBlock = new VmTextBlock();
            var lines = text?.Split('\n') ?? new string[0] { };

            foreach (var line in lines)
            {
                TextFormatClientTypeEnum type = TextFormatClientTypeEnum.unstyled;
                textBlock.Blocks.Add(new VmTextLine() { Text = line, Type = type, Key = GetUniqueKey(5) });
            }

            return textBlock;
        }

        private VmTextBlock DeserializeJson(string json)
        {
            VmTextBlock result = null;
            json = json?.Trim();

            try
            {
                if (IsValidJson(json))
                {
                    result = JsonConvert.DeserializeObject<VmTextBlock>(json);
                }
            }
            catch (JsonSerializationException ex)
            {
                throw new Exception($"Deserialize failed on JSON: {json}. {ex.Message}", ex);
            }

            return result ?? new VmTextBlock();
        }

        private string ParseToMarkdownFormat(TextFormatClientTypeEnum formatType)
        {
            var convertRules = new Dictionary<TextFormatClientTypeEnum, string>()
            {
                {TextFormatClientTypeEnum.orderedListItem, "{0}.{1}"},
                {TextFormatClientTypeEnum.unorderedListItem, "* {0}"},
                {TextFormatClientTypeEnum.unstyled,"{0}"} //text or line break
            };
            return convertRules.ContainsKey(formatType) ? convertRules[formatType] : string.Empty;
        }

        private bool IsValidJson(string json)
        {
            return (!string.IsNullOrWhiteSpace(json) && json.StartsWith("{") && json.EndsWith("}")) || (!string.IsNullOrWhiteSpace(json) && json.StartsWith("[") && json.EndsWith("]"));
        }

        private string GetUniqueKey(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(
                    Enumerable.Repeat(chars, length)
                              .Select(s => s[random.Next(s.Length)])
                              .ToArray());
        }

        private T ToEnum<T>(string str)
        {
            var enumType = typeof(T);
            foreach (var name in Enum.GetNames(enumType))
            {
                var enumMemberAttribute = ((System.Runtime.Serialization.EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(System.Runtime.Serialization.EnumMemberAttribute), true)).Single();
                if (enumMemberAttribute.Value == str) return (T)Enum.Parse(enumType, name);
            }
            return default(T);
        }
    }
}
