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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PTV.DataImport.WinExcelToJson.Tool;
using System;
using System.IO;

namespace PTV.DataImport.WinExcelToJson.Tasks
{
    internal static class LanguageCodeTask
    {
        private const string SourceExcelFile = "SourceFiles\\language_codes_270117.xlsx";
        private static readonly string GeneratedJsonFile = Path.Combine(AppSettings.OutputResourcesDir, "LanguageCodes.json");
        private const string DataSheetName = "Sheet0$";

        internal static void GenerateJsonFile()
        {
            ExcelReader excelRdr = new ExcelReader(new ExcelReaderSettings()
            {
                File = LanguageCodeTask.SourceExcelFile
            });

            // just a check that the sheet exists
            var sheetNameCheck = excelRdr.GetSheetNames().Find(name => string.Compare(name, LanguageCodeTask.DataSheetName, StringComparison.OrdinalIgnoreCase) == 0);

            if (string.IsNullOrWhiteSpace(sheetNameCheck))
            {
                throw new Exception($"The workbook doesn't contain a sheet with name: {LanguageCodeTask.DataSheetName}.");
            }

            // read the data rows to a list of anonym objects
            var data = excelRdr.ReadSheet(LanguageCodeTask.DataSheetName, reader =>
            {
                object dataItem = null;

                // check that we have a reader
                // we are checking the isdbnull(1) here to avoid the few empty rows that gets read from Excel sheet end
                if (reader != null && !reader.IsDBNull(1))
                {
                    // just creating the same kind of json object that existed in the initial languagecodes.json
                    dataItem = new
                    {
                        Code = reader.GetString(1),
                        Fi = reader.GetString(2),
                        Sv = reader.GetString(3),
                        En = reader.GetString(4),
                        Order = (int)reader.GetDouble(5)
                    };
                }

                return dataItem;
            });

            // convert data to json
            var json = JsonConvert.SerializeObject(data, new JsonSerializerSettings() { Formatting = Formatting.Indented, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            // overwrite the file always
            File.WriteAllText(GeneratedJsonFile, json);
        }
    }
}
