using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DbAnonymizer.models;
using Npgsql;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;

namespace DbAnonymizer
{
    enum ColumnType
    {
        CreatedBy,
        ModifiedBy
    }
    
    
    class Program
    {
        private static IConfigurationRoot Configuration = null;
        private static string ConnectionString = null;
        private static readonly MailGenerator _mailGenerator = new MailGenerator();

        private static readonly SortedDictionary<string, string> _anonymizedMails = new SortedDictionary<string, string>();
        private static readonly SortedDictionary<(string, ColumnType), List<string>> _tablesToUpdate = new SortedDictionary<(string, ColumnType), List<string>>();
        
        static void Main()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            ConnectionString = Configuration["ConnectionString"];

            Console.CursorVisible = false;
            var partialTimer = new Stopwatch();
            var totalTimer = new Stopwatch();
            totalTimer.Start();

            // get list of tables
            Helper.WriteLine("Fetching list of tables to update ...", ConsoleColor.Yellow);
            partialTimer.Start();
            var tables = GetTableNames();
            partialTimer.Stop();
            Helper.WriteLine($"Tables to update fetched in {partialTimer.Elapsed.Duration()}", ConsoleColor.Yellow);

            // process mails for update
            Helper.WriteLine("Processing mails to update ...", ConsoleColor.Yellow);
            partialTimer.Restart();
            foreach (var table in tables)
            {
                Console.WriteLine();
                Console.Write($"{table.Name}");

                if (!table.CreatedBy && !table.ModifiedBy)
                {
                    Helper.Write(" ... neither CreatedBy nor ModifiedBy field", ConsoleColor.Red);
                    continue;
                }

                if (table.CreatedBy) ProcessTableColumn(table.Name, ColumnType.CreatedBy);
                if (table.ModifiedBy) ProcessTableColumn(table.Name, ColumnType.ModifiedBy);
            }

            partialTimer.Stop();
            Console.WriteLine();
            Helper.WriteLine($"Mails to update processed in {partialTimer.Elapsed.Duration()}", ConsoleColor.Yellow);

            // anonymize emails in tables - CreatedBy, UpdatedBy
            var totalTableTimer = new Stopwatch();
            totalTableTimer.Start();
            var counterTableTotal = 1;
            const int dotsLimit = 3;
            foreach (var ((tableName, columnType), value) in _tablesToUpdate)
            {
                partialTimer.Restart();
                var mailsToAnonymize = value.Count;
                Console.Write($"Anonymizing table '{tableName}', column '{columnType.ToString()}' [{mailsToAnonymize} emails to anonymize]: ");

                var processingMessage = string.Empty;
                Helper.AddToProcessingMessage(ref processingMessage, "processing ");
                var command = PrepareUpdateCommand(tableName, columnType, value);
                var task = Task.Factory.StartNew(() => ExecuteCommand(command));

                var dotsCounter = 0;
                while (true)
                {
                    if (dotsCounter++ == dotsLimit)
                    {
                        Helper.RemoveFromProcessingMessage(ref processingMessage, dotsLimit);
                        dotsCounter = 0;
                    }
                    else
                    {
                        Helper.AddToProcessingMessage(ref processingMessage, ".");
                    }

                    if (task.IsCompleted) break;
                    Thread.Sleep(1000);
                }

                partialTimer.Stop();
                Helper.CleanLineText(processingMessage.Length);
                Console.WriteLine($"... anonymized in {partialTimer.Elapsed.Duration()}");
                
                Console.WriteLine($"[Partial duration: {totalTableTimer.Elapsed.Duration()}, update {counterTableTotal++}/{_tablesToUpdate.Count}]");
            }

            totalTableTimer.Stop();
            Helper.WriteLine($"All tables anonymized in ${totalTableTimer.Elapsed.Duration()}", ConsoleColor.Yellow);

            // handle Versioning table
            var versioningList = GetVersioningWithMail().ToList();
            var counterVersioning = 1;
            var statusVersioning = "";
            var versioningToAnonymize = versioningList.Count;
            partialTimer.Restart();
            Console.Write($"Updating 'Versioning' table ... [{versioningToAnonymize} records to anonymize] ... ");
            foreach (var versioning in versioningList)
            {
                Console.SetCursorPosition(Console.CursorLeft - statusVersioning.Length, Console.CursorTop);
                statusVersioning = $"{counterVersioning++}/{versioningToAnonymize}";
                Console.Write(statusVersioning);
                     
                var meta = JsonConvert.DeserializeObject<MetaData>(versioning.Meta);
                var languagesMetadata = meta.LanguagesMetaData;
                
                foreach (var lm in languagesMetadata)
                {
                    if (!string.IsNullOrEmpty(lm.ReviewedBy)) lm.ReviewedBy = GetAnonymizedMail(lm.ReviewedBy);
                    if (!string.IsNullOrEmpty(lm.ArchivedBy)) lm.ArchivedBy = GetAnonymizedMail(lm.ArchivedBy);
                }
            
                var metaString = JsonConvert.SerializeObject(meta);
                UpdateVersioning(versioning.Id, metaString);
            }
            
            partialTimer.Stop();
            Console.WriteLine();
            Helper.WriteLine($"'Versioning' table anonymized in ${partialTimer.Elapsed.Duration()}", ConsoleColor.Yellow);

            totalTimer.Stop();
            Helper.WriteLine($"Total anonymize time: ${totalTimer.Elapsed.Duration()}", ConsoleColor.Yellow);

            // store unique emails
            Helper.WriteLine($"{_anonymizedMails.Count} unique emails found.", ConsoleColor.DarkGreen);
            var emails = JsonConvert.SerializeObject( _anonymizedMails, Formatting.Indented);
            File.WriteAllText($"anonymized-emails-{DateTime.Now:yyyyMMddHHmm}.json", emails);
        }

        private static IEnumerable<T> ExecuteQuery<T>(string query)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
            return connection.Query<T>(query);
        }

        private static IEnumerable<Table> GetTableNames()
        {
            return ExecuteQuery<Table>(@"
select 
	tbl.tablename as ""Name"", 
	case when cb.column_name is null then false else true end as ""CreatedBy"",
	case when mb.column_name is null then false	else true end as ""ModifiedBy""
from pg_catalog.pg_tables tbl
left join information_schema.columns cb on tbl.tablename = cb.table_name and cb.table_schema = 'public' and cb.column_name = 'CreatedBy'
left join information_schema.columns mb on tbl.tablename = mb.table_name and mb.table_schema = 'public' and mb.column_name = 'ModifiedBy'
where schemaname = 'public'
order by tbl.tablename 
");
        }

        private static IEnumerable<string> GetEmails(string tableName, string columnName)
        {
            return ExecuteQuery<string>($"select distinct \"{columnName}\" from \"{tableName}\" where \"{columnName}\" like '%@%'");
        }

        private static void ProcessTableColumn(string tableName, ColumnType columnType)
        {
           var mails = GetEmails(tableName, columnType.ToString()).ToList();
           
           if (!mails.Any())
           {
              Helper.Write($" .. no mail for {columnType.ToString()}", ConsoleColor.Blue);
              return;
           }

           Console.Write($" .. {mails.Count} unique {columnType.ToString()} mails");

           _tablesToUpdate.Add((tableName, columnType), new List<string>());
           foreach (var mail in mails)
           {
               if (!_anonymizedMails.ContainsKey(mail))
               {
                   _anonymizedMails.Add(mail, _mailGenerator.GetUniqueMail());    
               }
               _tablesToUpdate[(tableName, columnType)].Add(mail);
           }
        }

        private static IEnumerable<Versioning> GetVersioningWithMail()
        {
            return ExecuteQuery<Versioning>(@"select ""Id"", ""Meta"" from ""Versioning"" where ""Meta"" like '%@%'");
        }

        private static string GetAnonymizedMail(string mail)
        {
            if (string.IsNullOrEmpty(mail)) return null;
            if (!_anonymizedMails.ContainsKey(mail)) _anonymizedMails.Add(mail, _mailGenerator.GetUniqueMail());
            return _anonymizedMails[mail];
        }

        private static void UpdateVersioning(Guid id, string meta)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Execute($"update \"Versioning\" set \"Meta\" = '{meta}' where \"Id\" = '{id}'");
        }

        private static void UpdateTableEmail(string tableName, ColumnType columnType, string mail)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            // var updateString = $"update \"{tableName}\" set \"{columnType.ToString()}\" = '{anonymizedMails[mail]}' where \"{columnType.ToString()}\" = '{mail}'";
            // File.AppendAllText("d:\\email-update-commands.txt", updateString);
            connection.Execute( $"update \"{tableName}\" set \"{columnType.ToString()}\" = '{_anonymizedMails[mail]}' where \"{columnType.ToString()}\" = '{mail}'", commandTimeout: 1500);
        }

        private static void ExecuteCommand(string command)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Execute(command, commandTimeout: 1500);
        }

        private static string PrepareUpdateCommand(string tableName , ColumnType column, IEnumerable<string> mails)
        {
            var values = mails.Select(mail => $"('{mail}','{_anonymizedMails[mail]}')").ToList();
            var valuesAsStr = string.Join(",", values);
            return $"update \"{tableName}\" set \"{column.ToString()}\" = mails.anonymized_mail from ( values {valuesAsStr} ) as  mails (original_mail, anonymized_mail) where \"{column.ToString()}\" = mails.original_mail";
        }

        private static Dictionary<string, int> CheckDuplicities()
        {
            var mailsStr = File.ReadAllText("d:\\anonymized-emails-prod1.json");
            var mails = JsonConvert.DeserializeObject<SortedDictionary<string, string>>(mailsStr);

            return mails.Values
                .GroupBy(k => k)
                .ToDictionary(k => k.Key, v => v.Count())
                .Where(pair => pair.Value > 1)
                .ToDictionary(k => k.Key, v => v.Value);
        }

        private static void AdditionalUpdate1()
        {
            var mails = File.ReadAllText("d:\\anonymized-emails-prod.json");
            var anonymizedMailsJson = JObject.Parse(mails);
            foreach (var x in anonymizedMailsJson)
            {
                _anonymizedMails.Add(x.Key, x.Value.Value<string>());
            }

            var counter = 1;
            var total = _anonymizedMails.Count;
            foreach (var mail in _anonymizedMails)
            {
                Console.WriteLine($"{counter++}/{total}: {mail.Key}");
                UpdateTableEmail("WebpageChannelUrl", ColumnType.CreatedBy, mail.Key);
                UpdateTableEmail("WebpageChannelUrl", ColumnType.ModifiedBy, mail.Key);
            }

            Console.WriteLine("fertig ...");
        }
    }
}