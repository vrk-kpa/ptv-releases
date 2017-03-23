using Newtonsoft.Json;
using PTV.DataImport.WinExcelToJson.Tool;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PTV.DataImport.WinExcelToJson.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace PTV.DataImport.WinExcelToJson.Tasks
{
    internal class GeneralDescriptionTask
    {
        private const string GeneralDescriptionsStartDataFile = "SourceFiles\\general_descriptions_170317.xlsx";
        private readonly string GeneralDescriptionsGeneratedFile = "GeneralDescription.json";

        private List<LanguageText> dataSheetNames = new List<LanguageText>
        {
            new LanguageText {Label = "Fi", Lang = "fi"},
            new LanguageText {Label = "SV", Lang = "sv"},
            new LanguageText {Label = "EN", Lang = "en"},
        };

        public void GenerateJsonFile()
        {
            FileInfo existingFile = new FileInfo(GeneralDescriptionsStartDataFile);
            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                var sheetNames = package.Workbook.Worksheets.Select(x => x.Name).ToList();
                Console.WriteLine(
                    $"Loading sheets {string.Join(", ", sheetNames)} in {GeneralDescriptionsStartDataFile} ");

                //package.Load(new FileStream(existingFile.FullName, FileMode.Open));
                // get the first worksheet in the workbook
                Dictionary<string, GeneralDescriptionJson> data = new Dictionary<string, GeneralDescriptionJson>();

                foreach (var dataSheet in dataSheetNames)
//                    foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                {
                    Console.WriteLine($"Generating {dataSheet.Label} for {dataSheet.Lang}.");

                    // just a check that the sheet exists

                    int index = 0;
                    bool defaultData = dataSheet.Lang == "fi";
                    bool readData = true;

                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name.ToLower() == dataSheet.Label.ToLower());
                    // check that we have a reader
                    // we are checking the isdbnull(1) here to avoid the few empty rows that gets read from Excel sheet end
                    if (worksheet != null)
                    {
                        try
                        {
                            do
                            {
                                int row = index + 4;
                                Console.WriteLine($"Row {index++} {row }:");
                                var referenceCode = GetString(worksheet, 0, row);
                                readData = !string.IsNullOrEmpty(referenceCode);
                                if (!readData)
                                {
                                    continue;
                                }

                                GeneralDescriptionJson dataItem;
                                if (!data.TryGetValue(referenceCode, out dataItem))
                                {
                                    dataItem = new GeneralDescriptionJson();
                                    data.Add(referenceCode, dataItem);
                                }

                                dataItem.ReferenceCode = referenceCode;
                                if (defaultData)
                                {
                                    dataItem.ServiceType = GetStringIfEmpty(worksheet, 1, row, dataItem.ServiceType);
                                    dataItem.ChargeType = GetStringIfEmpty(worksheet, 10, row, dataItem.ChargeType);

                                    ProcessTerms(GetString(worksheet, 15, row), dataItem.TargetGroup);
                                    ProcessTerms(GetString(worksheet, 16, row), dataItem.ServiceClass);
                                    ProcessTerms(GetString(worksheet, 17, row), dataItem.OntologyTerm);
                                    ProcessTerms(GetString(worksheet, 18, row), dataItem.LifeEvent);
                                    ProcessTerms(
                                        GetString(worksheet, 19, row), 
                                        dataItem.IndustrialClass,
                                        x => new KeyValuePair<string, string>(x.Key.Replace(".html", string.Empty), x.Value)
//                                        ,"http://www.stat.fi/meta/luokitukset/toimiala/001-2008/"
                                        );
                                    LoadLaws(worksheet, dataItem.Laws, dataSheet.Lang, row);
                                }
                                else
                                {
                                    UpdateLaws(worksheet, dataItem.Laws, dataSheet.Lang, row);
                                }

                                dataItem.Name.AddText(GetString(worksheet, 3, row), dataSheet.Lang);
                                dataItem.ShortDescription.AddText(GetString(worksheet, 4, row), dataSheet.Lang);
                                dataItem.Description.AddText(GetString(worksheet, 5, row), dataSheet.Lang);

                                dataItem.ServiceRestrictions.AddText(GetString(worksheet, 8, row), dataSheet.Lang);
                                dataItem.UserInstructions.AddText(GetString(worksheet, 9, row), dataSheet.Lang);
                                dataItem.ChargeTypeAdditionalInfo.AddText(GetString(worksheet, 11, row),
                                    dataSheet.Lang);
                                dataItem.DeadLineAdditionalInfo.AddText(GetString(worksheet, 12, row), dataSheet.Lang);
                                dataItem.ProcessingTimeAdditionalInfo.AddText(GetString(worksheet, 13, row),
                                    dataSheet.Lang);
                                dataItem.ValidityTimeAdditionalInfo.AddText(GetString(worksheet, 14, row),
                                    dataSheet.Lang);

//                            return dataItem;
                            } while (readData);
                        }
                        catch (InvalidCastException e)
                        {
                            Console.WriteLine($"Row { index } -> {e.Message}");
                        }
                        finally
                        {
                            Console.WriteLine();
                        }

                    }
                    else
                    {
                        throw new Exception($"The workbook doesn't contain a sheet with name: {dataSheet.Label}.");
                    }

                }
                //         convert data to json
                var json = JsonConvert.SerializeObject(data.Values.OrderBy(x => x.ReferenceCode), Formatting.Indented,
                    new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});

                var outputFile = new FileInfo(Path.Combine(AppSettings.OutputDir, GeneralDescriptionsGeneratedFile));

                // overwrite the file always
                File.WriteAllText(outputFile.FullName, json);
            }
        }

    private string GetString(ExcelWorksheet reader, int index, int row)
        {
            Console.Write($"{index}; ");
            string value = (reader.Cells[row, index + 1].Value as string)?.Trim();
            return value;
        }
        private string GetStringIfEmpty(ExcelWorksheet reader, int index, int row, string current)
        {
            if (string.IsNullOrEmpty(current))
            {
                return GetString(reader, index, row);
            }
            return current;
        }


        private void ProcessTerms(string data, List<KeyValuePair<string, string>> terms, Func<KeyValuePair<string, string>, KeyValuePair<string, string>> updateFunc = null)
        {
            if (string.IsNullOrEmpty(data))
            {
                return;
            }

//            List<KeyValuePair<string, string>> terms = new List<KeyValuePair<string, string>>();
            var pattern = @"([\s\S]*?)\s*\[(.+?)[\]\s]";

            foreach (Match match in Regex.Matches(data, pattern, RegexOptions.Multiline))
            {
                var name = match.Groups[2].Value.Replace("[", "").Replace("]", "").Trim();
                var value = match.Groups[1].Value.Replace("[", "").Replace("]", "").Trim();

                if (terms.All(x => x.Key != name))
                {
                    var keyValue = new KeyValuePair<string, string>($"{name}", value);
                    terms.Add(updateFunc?.Invoke(keyValue) ?? keyValue);
                }
            }

//            return terms;
        }

        private void LoadLaws(ExcelWorksheet reader, List<Law> lawsList, string language, int row)
        {
            List<Law> result = new List<Law>();
            var laws = ProcessLaws(GetString(reader, 6, row));
            var lawLinks = ProcessLaws(GetString(reader, 7, row));
            if (laws == null)
            {
                return;
            }
            while (laws.Count > 0)
            {
                var link = lawLinks.Dequeue();
                var reference = link;

                Law law = new Law();
                result.Add(law);

                law.Names.Add(new LanguageText(laws.Dequeue(), language));
                law.Links.Add(new LanguageText(link, language));
                law.LawReference = reference;
            }
            lawsList?.AddRange(result);
        }

        private void UpdateLaws(ExcelWorksheet reader, List<Law> lawsList, string language, int row)
        {
            List<Law> result = new List<Law>();
            var laws = ProcessLaws(GetString(reader, 6, row));
            var lawLinks = ProcessLaws(GetString(reader, 7, row));
            if (laws == null)
            {
                return;
            }
            if (lawLinks.Count != lawsList.Count)
            {
                if (lawLinks.Count == 0)
                {
                    Console.WriteLine($"Missing laws for {language}, source has {lawsList.Count} of laws.");
                    return;
                }
                Console.Error.WriteLine($"Wrong list of laws for languages. {language} contains {lawLinks.Count} instead of {lawsList.Count}.");
                return;
            }

            foreach (var law in lawsList)
            {
                var link = lawLinks.Dequeue();
                law.Names.Add(new LanguageText(laws.Dequeue(), language));
                law.Links.Add(new LanguageText(link, language));
            }
        }

        private Queue<string> ProcessLaws(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            return new Queue<string>(data.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
