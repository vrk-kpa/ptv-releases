using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.Migrations
{
    public class MigrateHelper
    {
        public void AddSqlScripts(MigrationBuilder migrationBuilder, string path)
        {
            var directory = new DirectoryInfo(path);
            if (directory.Exists)
            {
                foreach (var file in directory.GetFiles("*.sql").OrderBy(x => x.Name))
                {
                    Console.WriteLine($"Running script {file.Name}. Full path {file.FullName}.");
                    migrationBuilder.Sql(File.ReadAllText(file.FullName));
                }

            }
        }

        public void AddSqlScript(MigrationBuilder migrationBuilder, string path)
        {
            var file = new FileInfo(path);
            if (file.Exists)
            {
                Console.WriteLine($"Running script {file.Name}. Full path {file.FullName}.");
                migrationBuilder.Sql(File.ReadAllText(file.FullName));
            }
            else
            {
                Console.Error.WriteLine($"Script file {file.Name} not found. Full path {file.FullName}.");
            }
        }
    }
}