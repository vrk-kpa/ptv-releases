using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PTV.DataImport.WinExcelToJson.Tool;
using System;
using System.IO;

namespace PTV.DataImport.WinExcelToJson.Tasks
{
    internal static class LanguageCodeTask
    {
        private const string SourceExcelFile = "SourceFiles\\language codes 03062016 FINAL.xlsx";
        private static readonly string GeneratedJsonFile = Path.Combine(AppSettings.OutputDir, "LanguageCodes.json");
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
                        English = reader.GetString(11)
                    };
                }

                return dataItem;
            });

            // convert data to json
            var json = JsonConvert.SerializeObject(data, new JsonSerializerSettings() { Formatting = Formatting.Indented, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            // overwrite the file always
            File.WriteAllText(LanguageCodeTask.GeneratedJsonFile, json);
        }
    }
}
