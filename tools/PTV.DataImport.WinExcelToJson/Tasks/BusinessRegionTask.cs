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
using PTV.DataImport.WinExcelToJson.Model;
using PTV.DataImport.WinExcelToJson.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTV.DataImport.WinExcelToJson.Tasks
{
    internal static class BusinessRegionTask
    {
        private const string BusinessRegionStartDataFile = "SourceFiles\\company_regions_municipalities-23-03-2017.xlsx";
        private static readonly string BusinessRegionGeneratedFile = Path.Combine(AppSettings.OutputResourcesDir, "BusinessRegion.json");
        private const string DataSheetName = "'Business_regions_to_PTV$'";

        /// <summary>
        /// Generates a list of business regions in JSON format to applications file generation folder.
        /// </summary>
        /// <exception cref="System.Exception">The workbook doesn't contain the expected sheet name.</exception>
        internal static void GenerateJsonFile()
        {
            ExcelXmlReader reader = new ExcelXmlReader();

            reader.ReadSheets(BusinessRegionStartDataFile, worksheets =>
            {
                if (worksheets.Count == 0)
                {
                    throw new Exception("The workbook doesn't contain any sheet.");
                }

                ArrayList municipalities = new ArrayList();
                int rowIndex = 2;
                var worksheet = worksheets[1];
                bool keepReading = true;
                Dictionary<string, Area> businessAreas = new Dictionary<string, Area>();
                
                do
                {
                    Console.WriteLine($"Row {rowIndex}:");     
                    var municipalityNumber = worksheet.Cells[rowIndex, 2].Value;

                    if (municipalityNumber != null)
                    {
                        var municipalityCode = municipalityNumber.ToString().PadLeft(3, '0').Trim();
                        string businessCode = reader.GetString(worksheet, 1, rowIndex);

                        if (businessAreas.TryGetValue(businessCode, out Area businessArea))
                        {
                            businessArea.Municipalities.Add(municipalityCode);
                        }
                        else
                        {
                            businessArea = new Area
                            {
                                Code = businessCode,
                                Names = new List<LanguageName>
                                {
                                    new LanguageName
                                    {
                                        Language = "fi",
                                        Name = reader.GetString(worksheet, 3, rowIndex)
                                    },
                                    new LanguageName
                                    {
                                        Language = "sv",
                                        Name = reader.GetString(worksheet, 5, rowIndex)
                                    },
                                    new LanguageName
                                    {
                                        Language = "en",
                                        Name = reader.GetString(worksheet, 7, rowIndex)
                                    }
                                },
                                Municipalities = new List<string> { municipalityCode }
                            };
                            businessAreas.Add(businessCode, businessArea);
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
                var jsonResult = JsonConvert.SerializeObject(businessAreas.Values, Formatting.Indented);
                // overwrite the file always
                File.WriteAllText(BusinessRegionGeneratedFile, jsonResult);
            });
        }

    }
}
