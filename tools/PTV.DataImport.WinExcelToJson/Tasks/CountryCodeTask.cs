using Newtonsoft.Json;
using PTV.DataImport.WinExcelToJson.Tool;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTV.DataImport.WinExcelToJson.Tasks
{
    internal static class CountryCodeTask
    {
        private const string CountryCodeStartDataFile = "SourceFiles\\CountryCodes-06-01-2017.xlsx";
        private static readonly string CountryCodeGeneratedFile = Path.Combine(AppSettings.OutputDir, "CountryCodes.json");
        private const string DataSheetName = "'Country codes to PTV$'";

        /// <summary>
        /// Generates a list of country codes in JSON format to applications file generation folder.
        /// </summary>
        /// <exception cref="System.Exception">The workbook doesn't contain the expected sheet name.</exception>
        internal static void GenerateJsonFile()
        {
            ExcelReader excelRdr = new ExcelReader(new ExcelReaderSettings()
            {
                File = CountryCodeTask.CountryCodeStartDataFile
            });

            // just a check that the sheet exists
            var sheetNameCheck = excelRdr.GetSheetNames().Find(name => string.Compare(name, CountryCodeTask.DataSheetName, StringComparison.OrdinalIgnoreCase) == 0);

            if (string.IsNullOrWhiteSpace(sheetNameCheck))
            {
                throw new Exception($"The workbook doesn't contain a sheet with name: {CountryCodeTask.DataSheetName}.");
            }

            var data = excelRdr.ReadSheet(CountryCodeTask.DataSheetName, reader =>
            {
                object dataItem = null;

                // check that we have a reader
                // we are checking the isdbnull(1) here to avoid the few empty rows that gets read from Excel sheet end
                if (reader != null && !reader.IsDBNull(1))
                {
                    dataItem = new
                    {
                        names = new[]
                        {
                            new {
                                language = "en",
                                name = reader.GetString(0)
                            },
                            new {
                                language = "fi",
                                name = reader.GetString(1)
                            },
                            new {
                                language = "sv",
                                name = reader.GetString(2)
                            }
                        },
                        code = reader.GetString(3),
                        dialCodes = GetDialCodes(reader)
                    };
                }

                return dataItem;
            });

            // convert data to json
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            // overwrite the file always
            File.WriteAllText(CountryCodeTask.CountryCodeGeneratedFile, json);
        }

        private static string[] GetDialCodes(DbDataReader reader)
        {
            var dialCodesString = reader.GetValue(5)?.ToString().Trim();
            if (string.IsNullOrEmpty(dialCodesString))
            {
                return new string[] { };
            }

            return dialCodesString.Split(',')?.Select(x => string.Concat('+', x)).ToArray();
        }
    }
}
