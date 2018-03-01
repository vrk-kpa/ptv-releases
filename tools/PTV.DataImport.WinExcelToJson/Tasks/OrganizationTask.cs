using Newtonsoft.Json;
using PTV.DataImport.WinExcelToJson.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTV.DataImport.WinExcelToJson.Tasks
{
    internal static class OrganizationTask
    {
        private const string OrganizationsStartDataFile = "SourceFiles\\orgToImportAll.xlsx";
        private static readonly string OrganizationsGeneratedFile = Path.Combine(AppSettings.OutputDir, "OrganizationsAdditional.json");
        private const string DataSheetName = "Organisaatiot$";

        /// <summary>
        /// Generates a list of organizations in JSON format to applications file generation folder.
        /// </summary>
        /// <exception cref="System.Exception">The workbook doesn't contain the expected sheet name.</exception>
        internal static void GenerateJsonFile()
        {
            ExcelReader excelRdr = new ExcelReader(new ExcelReaderSettings()
            {
                File = OrganizationTask.OrganizationsStartDataFile
            });

            // just a check that the sheet exists
            var sheetNameCheck = excelRdr.GetSheetNames().Find(name => string.Compare(name, OrganizationTask.DataSheetName, StringComparison.OrdinalIgnoreCase) == 0);

            if (string.IsNullOrWhiteSpace(sheetNameCheck))
            {
                throw new Exception($"The workbook doesn't contain a sheet with name: {OrganizationTask.DataSheetName}.");
            }

            var data = excelRdr.ReadSheet(OrganizationTask.DataSheetName, reader =>
            {
                object dataItem = null;

                // check that we have a reader
                // we are checking the isdbnull(1) here to avoid the few empty rows that gets read from Excel sheet end
                if (reader != null && !reader.IsDBNull(1))
                {
                    dataItem = new
                    {
                        businessId = (reader.GetValue(0) as string)?.Trim().Replace(" ", string.Empty),
                        name = reader.GetString(1).Trim(),
                        organizationType = reader.GetString(2).Trim()
                    };
                }

                return dataItem;
            });

            // convert data to json
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            // overwrite the file always
            File.WriteAllText(OrganizationTask.OrganizationsGeneratedFile, json);
        }
    }
}
