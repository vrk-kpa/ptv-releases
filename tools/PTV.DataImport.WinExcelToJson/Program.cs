using PTV.DataImport.WinExcelToJson.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTV.DataImport.WinExcelToJson
{
    class Program
    {
        private const string ReminderMessage = "Remember to copy the generated files to PTV.DataImport.ConsoleApp Generated folder if you changed the default output directory!";

        static void Main(string[] args)
        {
            try
            {
                ProgramMenuOption userMenuAction = ProgramMenuOption.Exit;

                do
                {
                    userMenuAction = DisplayProgramMenu();

                    switch (userMenuAction)
                    {
                        case ProgramMenuOption.All:
                            Stopwatch swTotal = new Stopwatch();
                            swTotal.Start();

                            // general description
                            GenerateGeneralDescription();

                            // language code
                            GenerateLanguageCode();

                            // municipality
                            GenerateMunicipality();

                            swTotal.Stop();
                            string msg = $"All JSON files generated. Total time: {swTotal.Elapsed}";
                            Console.WriteLine(msg);
                            Console.WriteLine();
                            ShowReminder();
                            break;
                        case ProgramMenuOption.GeneralDescription:
                            GenerateGeneralDescription();
                            ShowReminder();
                            break;
                        case ProgramMenuOption.LanguageCode:
                            GenerateLanguageCode();
                            ShowReminder();
                            break;
                        case ProgramMenuOption.Municipality:
                            GenerateMunicipality();
                            ShowReminder();
                            break;
                        case ProgramMenuOption.Organization:
                            GenerateOrganization();
                            ShowReminder();
                            break;
                        case ProgramMenuOption.Exit:
                            break;
                        default:
                            // invalid options
                            userMenuAction = ProgramMenuOption.Exit;
                            break;
                    }
                } while (userMenuAction != ProgramMenuOption.Exit);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Done.");
            Console.Read();
        }

        private static void EnsureOutputDirectoryExists()
        {
            string od = AppSettings.OutputDir;

            if (!Directory.Exists(od))
            {
                Console.WriteLine($"Output directory '{od}' doesn't exist. Creating the directory.");
                Directory.CreateDirectory(od);
            }
        }

        private static void GenerateGeneralDescription()
        {
            EnsureOutputDirectoryExists();

            Console.WriteLine("Generating general description JSON..");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            GeneralDescriptionTask.GenerateJsonFile();
            sw.Stop();
            string msg = $"General description JSON generated in {sw.Elapsed}.";
            Console.WriteLine(msg);
            Console.WriteLine();
        }

        private static void GenerateLanguageCode()
        {
            EnsureOutputDirectoryExists();

            Console.WriteLine("Generating language code JSON..");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            LanguageCodeTask.GenerateJsonFile();
            sw.Stop();
            string msg = $"Language code JSON generated in {sw.Elapsed}.";
            Console.WriteLine(msg);
            Console.WriteLine();
        }

        private static void GenerateMunicipality()
        {
            EnsureOutputDirectoryExists();

            Console.WriteLine("Generating municipality JSON..");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            MunicipalityTask.GenerateJsonFile();
            sw.Stop();
            string msg = $"Municipality JSON generated in {sw.Elapsed}.";
            Console.WriteLine(msg);
            Console.WriteLine();
        }

        private static void GenerateOrganization()
        {
            EnsureOutputDirectoryExists();

            Console.WriteLine("Generating organization JSON..");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            OrganizationTask.GenerateJsonFile();
            sw.Stop();
            string msg = $"Organization JSON generated in {sw.Elapsed}.";
            Console.WriteLine(msg);
            Console.WriteLine();
        }

        private static ProgramMenuOption DisplayProgramMenu()
        {
            Console.WriteLine();
            Console.WriteLine("PTV Excel to JSON. Enter the action number and press enter:");
            Console.WriteLine();

            var options = Enum.GetValues(typeof(ProgramMenuOption));

            foreach (var option in options)
            {
                var name = Enum.GetName(typeof(ProgramMenuOption), option);
                var value = (int)option;

                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Write("{0}. {1}", value, name);
                Console.ResetColor();
                Console.WriteLine($" - {Program.GetProgramMenuOptionDescription((ProgramMenuOption)value)}");
            }

            ProgramMenuOption result = ProgramMenuOption.Exit;
            var userAction = Console.ReadLine().Trim();
            Enum.TryParse(userAction, out result);

            Console.WriteLine();
            return result;
        }

        private static string GetProgramMenuOptionDescription(ProgramMenuOption option)
        {
            // quick and dirty descriptions for enum values

            string desc = "No description";

            switch (option)
            {
                case ProgramMenuOption.Exit:
                    desc = "Exit the application.";
                    break;
                case ProgramMenuOption.All:
                    desc = "Generate all JSON files from Excel files.";
                    break;
                case ProgramMenuOption.GeneralDescription:
                    desc = "Generate general descriptions JSON file from Excel file.";
                    break;
                case ProgramMenuOption.LanguageCode:
                    desc = "Generate language code JSON file from Excel file.";
                    break;
                case ProgramMenuOption.Municipality:
                    desc = "Generate municipallity JSON file from Excel file.";
                    break;
                case ProgramMenuOption.Organization:
                    desc = "Generate organization JSON file from Excel file.";
                    break;
                default:
                    break;
            }

            return desc;
        }

        private static void ShowReminder()
        {
            Console.WriteLine();
            Console.WriteLine(Program.ReminderMessage);
            Console.WriteLine();
        }
    }
}
