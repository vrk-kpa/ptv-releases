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
using System.IO;

namespace PTV.DataImport.ConsoleApp
{
    /// <summary>
    /// Contains fake ptv json file names.
    /// </summary>
    internal static class FakePtvJsonFileNames
    {
        /// <summary>
        /// Directory where the json files are.
        /// </summary>
        internal static readonly string Directory = Path.Combine("Generated", "FakePtv");
        /// <summary>
        /// Organizations json file name.
        /// </summary>
        internal const string Organizations = "organizations.json";
        /// <summary>
        /// Phone channels json file name.
        /// </summary>
        internal const string PhoneChannels = "phone_service.json";
        /// <summary>
        /// Services json file name.
        /// </summary>
        internal const string Services = "services.json";
        /// <summary>
        /// Transaction form channels json file name.
        /// </summary>
        internal const string TransactionFormChannels = "transactionservices.json";
        /// <summary>
        /// Service location channels json file name.
        /// </summary>
        internal const string ServiceLocationsChannel = "servicelocations.json";
        /// <summary>
        /// General descriptions json file name.
        /// </summary>
        internal const string GeneralDescriptions = "general_descriptions.json";
        /// <summary>
        /// eChannels json file name.
        /// </summary>
        internal const string ElectronicChannels = "electronictransactionservices.json";
        /// <summary>
        /// Webpage channels json file name.
        /// </summary>
        internal const string WebpageChannels = "electronicinformationservices.json";

        /// <summary>
        /// Concatenates the file name with the FakePtvJsonFileNames.Directory
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>path with file name</returns>
        internal static string GetFilePath(string fileName)
        {
            return Path.Combine(Directory, fileName);
        }
    }
}
