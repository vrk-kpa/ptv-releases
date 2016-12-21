using Newtonsoft.Json;
using PTV.DataImport.WinExcelToJson.Tool;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PTV.DataImport.WinExcelToJson.Tasks
{
    internal static class GeneralDescriptionTask
    {
        private const string GeneralDescriptionsStartDataFile = "SourceFiles\\general_descriptions_071216.xlsx";
        private static readonly string GeneralDescriptionsGeneratedFile = "D:\\GeneralDescription.json";
        private const string DataSheetName = "'Valmiit pohjakuvaukset (+meta)$'";

        public static void GenerateJsonFile()
        {
            ExcelReader excelRdr = new ExcelReader(new ExcelReaderSettings()
            {
                File = GeneralDescriptionTask.GeneralDescriptionsStartDataFile,
                SkipRows = 2,
                FirstRowContainsColumnNames = true
            });

            // just a check that the sheet exists
            var sheetNameCheck = excelRdr.GetSheetNames().Find(name => string.Compare(name, GeneralDescriptionTask.DataSheetName, StringComparison.OrdinalIgnoreCase) == 0);

            if (string.IsNullOrWhiteSpace(sheetNameCheck))
            {
                throw new Exception($"The workbook doesn't contain a sheet with name: {GeneralDescriptionTask.DataSheetName}.");
            }
            int index = 1;

            var data = excelRdr.ReadSheet(GeneralDescriptionTask.DataSheetName, reader =>
            {
                object dataItem = null;

                // check that we have a reader
                // we are checking the isdbnull(1) here to avoid the few empty rows that gets read from Excel sheet end
                if (reader != null && !reader.IsDBNull(1))
                {
                    try
                    {
                        Console.WriteLine($"Row {index++} {index + 2}:");
                        dataItem = new
                        {
                            ReferenceCode = GetString(reader, 0),
                            ServiceType = GetString(reader, 1),
                            Name = GetString(reader, 2),
                            ShortDescription = GetString(reader, 3),
                            Description = GetString(reader, 4),
//                            Laws = ProcessLaws(GetString(reader, 5)),
//                            LawLinks = ProcessLaws(GetString(reader, 6)),
//                            LawsSv = ProcessLaws(GetString(reader, 7)),
//                            LawsLinkSv = ProcessLaws(GetString(reader, 8)),
                            Laws = GetLaws(reader),
                            ServiceRestrictions = GetString(reader, 9),
                            UserInstructions = GetString(reader, 10),
                            ChargeType = GetString(reader, 11),
                            ChargeTypeAdditionalInfo = GetString(reader, 12),
                            TasksAdditionalInfo = GetString(reader, 13),
                            DeadLineAdditionalInfo = GetString(reader, 14),
                            ProcessingTimeAdditionalInfo = GetString(reader, 15),
                            ValidityTimeAdditionalInfo = GetString(reader, 16),
                            TargetGroup = ProcessTerms(GetString(reader, 17)),
                            ServiceClass = ProcessTerms(GetString(reader, 18)),
                            OntologyTerm = ProcessTerms(GetString(reader, 19)),
                            LifeEvent = ProcessTerms(GetString(reader, 20)),
                            IndustrialClass = ProcessTerms(GetString(reader, 21))
                        };
                    }
                    catch (InvalidCastException e)
                    {
                        Console.WriteLine($"Row {index + 2} -> {e.Message}");
                    }
                    finally
                    {
                        Console.WriteLine();
                    }
                }

                return dataItem;
            });

            // convert data to json
            var json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            // overwrite the file always
            File.WriteAllText(GeneralDescriptionTask.GeneralDescriptionsGeneratedFile, json);
        }

        private static string GetString(DbDataReader reader, int index)
        {
            string value = (reader.GetValue(index) as string)?.Trim();
            Console.Write($"{index}; ");
            return value;
        }


        private static List<KeyValuePair<string, string>> ProcessTerms(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            List<KeyValuePair<string, string>> terms = new List<KeyValuePair<string, string>>();
            var pattern = @"([\s\S]*?)\s*\[(.+?)[\]\s]";

            foreach (Match match in Regex.Matches(data, pattern, RegexOptions.Multiline))
            {
                var name = match.Groups[2].Value.Replace("[", "").Replace("]", "").Trim();
                var value = match.Groups[1].Value.Replace("[", "").Replace("]", "").Trim();

                terms.Add(new KeyValuePair<string, string>(name, value));
            }

            return terms;
        }

        private static List<Law> GetLaws(DbDataReader reader)
        {
            List<Law> result = new List<Law>();
            var laws = ProcessLaws(GetString(reader, 5));
            var lawLinks = ProcessLaws(GetString(reader, 6));
            var lawsSv = ProcessLaws(GetString(reader, 7));
            var lawsLinkSv = ProcessLaws(GetString(reader, 8));
            if (laws == null)
            {
                return null;
            }
            while (laws.Count > 0)
            {
                var law = new Law();
                law.LawFi = new LawDetail
                {
                    Name = laws.Dequeue(),
                    Link = lawLinks.Dequeue()
                };
                law.LawSv = new LawDetail
                {
                    Name = lawsSv.Dequeue(),
                    Link = lawsLinkSv.Dequeue()
                };
                result.Add(law);
            }
            return result;
        }
        private static Queue<string> ProcessLaws(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            return new Queue<string>(data.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
        }
    }

    internal class LawDetail
    {
        public string Name { get; set; }
        public string Link { get; set; }
    }

    internal class Law
    {
        public LawDetail LawFi { get; set; }
        public LawDetail LawSv { get; set; }
    }
}
