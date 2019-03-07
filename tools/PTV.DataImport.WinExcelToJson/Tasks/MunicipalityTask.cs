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
using PTV.DataImport.WinExcelToJson.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using PTV.DataImport.WinExcelToJson.Model;

namespace PTV.DataImport.WinExcelToJson.Tasks
{

    internal static class MunicipalityTask
    {
        private const string MunicipalitiesStartDataFile = "SourceFiles\\CUsersA011673.AHKDownloadsKuntaluettelo.xlsx";
        private static readonly string MunicipalitiesGeneratedFile = Path.Combine(AppSettings.OutputResourcesDir, "Municipality.json");
        private static readonly string ProvincesGeneratedFile = Path.Combine(AppSettings.OutputResourcesDir, "Province.json");
        private const string DataSheetName = "'Muncipalities to PTV$'";

        /// <summary>
        /// Generates a list of municipalities in JSON format to applications file generation folder.
        /// </summary>
        /// <exception cref="System.Exception">The workbook doesn't contain the expected sheet name.</exception>
        internal static void GenerateJsonFile()
        {
            ExcelXmlReader reader = new ExcelXmlReader();

            reader.ReadSheets(MunicipalitiesStartDataFile, worksheets =>
            {
                if (worksheets.Count == 0)
                {
                    throw new Exception("The workbook doesn't contain any sheet.");
                }
                ArrayList municipalities = new ArrayList();
                int rowIndex = 2;
                var worksheet = worksheets[1];
                bool keepReading = true;
                Dictionary<string, Province> provinces = new Dictionary<string, Province>();
                do
                {
                    Console.WriteLine($"Row {rowIndex}:");
                    var number = worksheet.Cells[rowIndex, 1].Value;
                    if (number != null)
                    {
                        var dataItem = new
                        {
                            municipalityCode = number.ToString().PadLeft(3, '0').Trim(),
                            names = new[]
                            {
                                new LanguageName(reader.GetString(worksheet, 2, rowIndex), "fi"),
                                new LanguageName(reader.GetString(worksheet, 3, rowIndex), "sv")
                            }
                        };
                        municipalities.Add(dataItem);
                        string provinceCode = reader.GetString(worksheet, 16, rowIndex);

                        if (provinces.TryGetValue(provinceCode, out Province province))
                        {
                            province.Municipalities.Add(dataItem.municipalityCode);
                        }
                        else
                        {
                            province = new Province
                            {
                                Code = provinceCode,
                                Names = new List<LanguageName>
                                {
                                    new LanguageName
                                    {
                                        Language = "fi",
                                        Name = reader.GetString(worksheet, 17, rowIndex)
                                    },
                                    new LanguageName
                                    {
                                        Language = "sv",
                                        Name = reader.GetString(worksheet, 18, rowIndex)
                                    }
                                },
                                Municipalities = new List<string> { dataItem.municipalityCode }
                            };
                            provinces.Add(provinceCode, province);
                        }
                    }
                    else
                    {
                        keepReading = false;
                    }
                    rowIndex++;
                    Console.WriteLine();
                } while (keepReading);

                // convert data to json
                var json = JsonConvert.SerializeObject(municipalities, Formatting.Indented);
                // overwrite the file always
                File.WriteAllText(MunicipalitiesGeneratedFile, json);       
                
                // convert data to json
                var jsonProvince = JsonConvert.SerializeObject(provinces.Values, Formatting.Indented);
                // overwrite the file always
                File.WriteAllText(ProvincesGeneratedFile, jsonProvince);
            });
        }

    }
}
