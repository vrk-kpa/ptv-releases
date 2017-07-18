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
