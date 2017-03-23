using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.Domain.Model.Models.OpenApi.Extensions
{
    /// <summary>
    /// Extensions for string type
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Set the value property length of the string as defined.
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="length">The length of value string</param>
        /// <returns></returns>
        public static string SetStringValueLength(this string value, int length)
        {
            return value?.Substring(0, value.Length > length ? length : value.Length);
        }

        /// <summary>
        /// Converts a string into language item list, value as a Finnish translation.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>A language item list</returns>
        public static List<VmOpenApiLanguageItem> ConvertToLanguageList(this string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return new List<VmOpenApiLanguageItem>() { new VmOpenApiLanguageItem() { Language = LanguageCode.fi.ToString(), Value = value } };
        }
    }
}
