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
using PTV.Database.Model.Models;
using PTV.DataImport.ConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.DataImport.ConsoleApp
{
    /// <summary>
    /// Contains general helper methods.
    /// </summary>
    public static class HelperExtensionMethods
    {
        /// <summary>
        /// Tries to find a source general description with the given entry id.
        /// </summary>
        /// <param name="srcGeneralDescriptions"></param>
        /// <param name="srcEntryId">source entry id for general description</param>
        /// <returns>null if not found otherwise the found name corresponding the entry id</returns>
        public static string GetNameFromSourceId(this List<SourceGeneralDescription> srcGeneralDescriptions, int srcEntryId)
        {
            string sourceName = null;

            if (srcGeneralDescriptions != null && srcGeneralDescriptions.Count > 0)
            {
                sourceName = srcGeneralDescriptions.Find(m => m.Id == srcEntryId)?.Name;
            }

            return sourceName;
        }

        /// <summary>
        /// Tries to find StatutoryServiceName from the list of PTV entities that matches the given fake PTV source entity id.
        /// </summary>
        /// <param name="serviceNames">list of StatutoryServiceNames from PTV database</param>
        /// <param name="srcGeneralDescriptions">list of SourceGeneralDescription from fake PTV source</param>
        /// <param name="srcEntryId">source entry id (reference to the id in SourceGeneralDescription list entity)</param>
        /// <returns></returns>
        internal static StatutoryServiceName GetStatutoryServiceName(this List<StatutoryServiceName> serviceNames, List<SourceGeneralDescription> srcGeneralDescriptions, int srcEntryId)
        {
            StatutoryServiceName foundServiceName = null;

            if (serviceNames != null && serviceNames.Count > 0 && srcGeneralDescriptions != null && srcGeneralDescriptions.Count > 0)
            {
                var name = srcGeneralDescriptions.GetNameFromSourceId(srcEntryId);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    foundServiceName = serviceNames.Find(m => string.Compare(m.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
                }
            }

            return foundServiceName;
        }
    }
}
