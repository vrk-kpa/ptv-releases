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

using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi.Extensions
{
    /// <summary>
    /// Extentions for list type
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Get's the Finnish value from language item list. If no Finnish value is found first one in the list is returned.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ValueString(this IList<VmOpenApiLanguageItem> list)
        {
            if (list == null)
                return null;

            if (list.Count == 0)
            {
                return null;
            }

            var item = list.FirstOrDefault(i => i.Language == DomainConstants.DefaultLanguage);

            if (item != null)
            {
                return item.Value;
            }

            return list.FirstOrDefault().Value;
        }

        /// <summary>
        /// Get's the Finnish value from TypeByLanguage list. If no Finnish value is found first one in the list is returned.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ValueString(this IList<VmOpenApiNameTypeByLanguage> list)
        {
            if (list == null)
                return null;

            if (list.Count == 0)
            {
                return null;
            }

            var item = list.FirstOrDefault(i => i.Language == DomainConstants.DefaultLanguage);

            if (item != null)
            {
                return item.Type;
            }

            return list.FirstOrDefault().Type;
        }

        /// <summary>
        /// Set the value property length of all the items in list as defined.
        /// </summary>
        /// <param name="list">Language list</param>
        /// <param name="length">The length of value string</param>
        /// <returns></returns>
        public static IList<VmOpenApiLanguageItem> SetListValueLength(this IList<VmOpenApiLanguageItem> list, int length = 2500)
        {
            if (list?.Count > 0)
            {
                list.ForEachNotNull(l => l.Value = GetStringByLength(l.Value, length));
            }

            return list;
        }

        /// <summary>
        /// Set the value property length of all the items in list as defined.
        /// </summary>
        /// <param name="list">Localized list</param>
        /// <param name="length">The lenght of value string</param>
        /// <returns></returns>
        public static IList<VmOpenApiLocalizedListItem> SetListValueLength(this IList<VmOpenApiLocalizedListItem> list, int length = 2500)
        {
            if (list?.Count > 0)
            {
                list.ForEachNotNull(l => l.Value = GetStringByLength(l.Value, length));
            }

            return list;
        }

        /// <summary>
        /// Check that the list does not contain multiple items with same language and type.
        /// </summary>
        /// <param name="list">Localized list</param>
        /// <returns></returns>
        public static IList<VmOpenApiLocalizedListItem> DisregardDuplicates(this IList<VmOpenApiLocalizedListItem> list)
        {
            var newList = new List<VmOpenApiLocalizedListItem>();
            list.ForEachNotNull(d =>
            {
                // Let's only add the item into newList if same type (and language) does not already exist in the new list.
                // POST method will fail if list contains several items with same type and language!
                if (newList.Where(i => i.Type == d.Type && i.Language == d.Language).FirstOrDefault() == null)
                {
                    newList.Add(d);
                }
            });

            return newList;
        }

        /// <summary>
        /// Returns language codes that exist in list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public static HashSet<string> GetAvailableLanguages(this HashSet<string> list, IList<VmOpenApiLocalizedListItem> vModel)
        {
            if (vModel == null) return list;

            vModel.ForEachNotNull(item =>
            {
                list.Add(item.Language);
            });

            return list;
        }

        /// <summary>
        /// Returns language codes that exist in list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public static HashSet<string> GetAvailableLanguages(this HashSet<string> list, IList<VmOpenApiLanguageItem> vModel)
        {
            vModel.ForEachNotNull(item =>
            {
                list.Add(item.Language);
            });

            return list;
        }

        /// <summary>
        /// Returns language codes that exist in list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public static HashSet<string> GetAvailableLanguages(this HashSet<string> list, IList<VmOpenApiNameTypeByLanguage> vModel)
        {
            vModel.ForEachNotNull(item =>
            {
                list.Add(item.Language);
            });

            return list;
        }

        /// <summary>
        /// Returns language codes that exist in legislation list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public static HashSet<string> GetAvailableLanguages(this HashSet<string> list, IList<V4VmOpenApiLaw> vModel)
        {
            vModel.ForEachNotNull(item =>
            {
                list.GetAvailableLanguages(item.Names);
                list.GetAvailableLanguages(item.WebPages);
            });

            return list;
        }

        /// <summary>
        /// Returns language codes that exist in web page list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public static HashSet<string> GetAvailableLanguages<TModel>(this HashSet<string> list, IList<TModel> vModel) 
            where TModel : class, IVmOpenApiWebPageVersionBase
        {
            vModel.ForEachNotNull(item =>
            {
                list.Add(item.Language);
            });

            return list;
        }

        /// <summary>
        /// Returns language codes that exist in email list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public static HashSet<string> GetAvailableLanguages(this HashSet<string> list, IList<V4VmOpenApiEmail> vModel)
        {
            vModel.ForEachNotNull(item =>
            {
                list.Add(item.Language);
            });

            return list;
        }

        /// <summary>
        /// Returns language codes that exist in phone list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public static HashSet<string> GetPhoneAvailableLanguages<TModel>(this HashSet<string> list, IList<TModel> vModel) 
            where TModel : class, IVmOpenApiPhoneSimpleVersionBase
        {
            vModel.ForEachNotNull(item =>
            {
                list.Add(item.Language);
            });

            return list;
        }

        /// <summary>
        /// Returns language codes that exist in address list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public static HashSet<string> GetAvailableLanguages(this HashSet<string> list, IList<V9VmOpenApiAddressIn> vModel)
        {
            vModel.ForEachNotNull(item =>
            {
                list.GetAvailableLanguages(item.StreetAddress?.Street);
                list.GetAvailableLanguages(item.StreetAddress?.AdditionalInformation);
                list.GetAvailableLanguages(item.PostOfficeBoxAddress?.PostOfficeBox);
                list.GetAvailableLanguages(item.PostOfficeBoxAddress?.AdditionalInformation);
                list.GetAvailableLanguages(item.OtherAddress?.AdditionalInformation);
                list.GetAvailableLanguages(item.ForeignAddress);
            });

            return list;
        }

        /// <summary>
        /// Returns language codes that exist in address list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public static HashSet<string> GetAvailableLanguages(this HashSet<string> list, IList<V8VmOpenApiAddressDeliveryIn> vModel)
        {
            vModel.ForEachNotNull(item =>
            {
                list.GetAvailableLanguages(item.StreetAddress?.Street);
                list.GetAvailableLanguages(item.StreetAddress?.AdditionalInformation);
                list.GetAvailableLanguages(item.PostOfficeBoxAddress?.PostOfficeBox);
                list.GetAvailableLanguages(item.PostOfficeBoxAddress?.AdditionalInformation);
                list.GetAvailableLanguages(item.DeliveryAddressInText);
                list.GetAvailableLanguages(item.FormReceiver);
            });

            return list;
        }

        /// <summary>
        /// Returns language codes that exist in address list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public static HashSet<string> GetAvailableLanguages(this HashSet<string> list, IList<V9VmOpenApiAddressLocationIn> vModel)
        {
            vModel.ForEachNotNull(item =>
            {
                list.GetAvailableLanguages(item.StreetAddress?.Street);
                list.GetAvailableLanguages(item.StreetAddress?.AdditionalInformation);
                list.GetAvailableLanguages(item.PostOfficeBoxAddress?.PostOfficeBox);
                list.GetAvailableLanguages(item.PostOfficeBoxAddress?.AdditionalInformation);
                list.GetAvailableLanguages(item.OtherAddress?.AdditionalInformation);
                list.GetAvailableLanguages(item.LocationAbroad);
            });

            return list;
        }

        /// <summary>
        /// Returns language codes that exist in electronic invoicing list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public static HashSet<string> GetAvailableLanguages(this HashSet<string> list, IList<VmOpenApiOrganizationEInvoicing> vModel)
        {
            vModel.ForEachNotNull(item =>
            {
                list.GetAvailableLanguages(item.AdditionalInformation);
            });

            return list;
        }

        /// <summary>
        /// Returns language codes that exist in service producer list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public static HashSet<string> GetAvailableLanguages(this HashSet<string> list, IList<V9VmOpenApiServiceProducerIn> vModel)
        {
            vModel.ForEachNotNull(item =>
            {
                list.GetAvailableLanguages(item.AdditionalInformation);
            });

            return list;
        }

        /// <summary>
        /// Returns language codes that exist in service voucher list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public static HashSet<string> GetAvailableLanguages(this HashSet<string> list, IList<V9VmOpenApiServiceVoucher> vModel)
        {
            vModel.ForEachNotNull(item =>
            {
                list.Add(item.Language);
            });

            return list;
        }

        /// <summary>
        /// Returns language codes that exist in accessibility classification list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public static HashSet<string> GetAvailableLanguages(this HashSet<string> list, IList<VmOpenApiAccessibilityClassification> vModel)
        {
            vModel.ForEachNotNull(item =>
            {
                list.Add(item.Language);
            });

            return list;
        }

        /// <summary>
        /// Returns language codes that exist in attachment list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public static HashSet<string> GetAvailableLanguages(this HashSet<string> list, IList<VmOpenApiAttachment> vModel)
        {
            vModel.ForEachNotNull(item =>
            {
                list.Add(item.Language);
            });

            return list;
        }

        /// <summary>
        /// Returns language codes that exist in attachment list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public static HashSet<string> GetAvailableLanguages(this HashSet<string> list, IList<V11VmOpenApiServiceHour> vModel)
        {
            vModel.ForEachNotNull(item =>
            {
                list.GetAvailableLanguages(item.AdditionalInformation);
            });

            return list;
        }

        /// <summary>
        /// Get's the Finnish value from accessibility classification list. If no Finnish item is found first one in the list is returned.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static VmOpenApiAccessibilityClassification GetSingle(this IList<VmOpenApiAccessibilityClassification> list)
        {
            if (list == null)
                return null;

            if (list.Count == 0)
            {
                return null;
            }

            var ac = list.FirstOrDefault(i => i.Language == DomainConstants.DefaultLanguage);

            if (ac != null)
            {
                return ac;
            }

            return list.FirstOrDefault();
        }

        private static string GetStringByLength(string str, int length)
            {
                if (string.IsNullOrEmpty(str))
                    return str;

                if (str.Length > length)
                {
                    return str.Substring(0, length);
                }

                return str;
            }
        }
}
