using System;
using System.IO;
using OfficeOpenXml;

namespace PTV.DataImport.WinExcelToJson.Tool
{
    internal class ExcelXmlReader
    {
        public void ReadSheets(string fileName, Action<ExcelWorksheets> readAction)
        {
            FileInfo existingFile = new FileInfo(fileName);
            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                if (package.Workbook.Worksheets.Count == 0)
                {
                    throw new Exception("The workbook doesn't contain any sheet.");
                }

                readAction?.Invoke(package.Workbook.Worksheets);
            }
        }

        public string GetString(ExcelWorksheet reader, int index, int row)
        {
            Console.Write($"{index}; ");
            string value = reader.Cells[row, index].Value?.ToString().Trim();
            return value;
        }

        public string GetStringIfEmpty(ExcelWorksheet reader, int index, int row, string current)
        {
            if (string.IsNullOrEmpty(current))
            {
                return GetString(reader, index, row);
            }
            return current;
        }
    }
}
