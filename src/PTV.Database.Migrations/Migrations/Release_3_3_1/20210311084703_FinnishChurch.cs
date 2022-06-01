using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;

namespace PTV.Database.Migrations.Migrations.Release_3_3_1
{
    public partial class FinnishChurch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Console.WriteLine($"{DateTime.UtcNow} - Changing Aland islands to Finnish church.");
            
            this.AddMigrationAction(serviceProvider =>
            {
                var contextManager = serviceProvider.GetService<IContextManager>();

                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var repo = unitOfWork.CreateRepository<IGeneralDescriptionTypeRepository>();
                    var alandIslands = repo.All().FirstOrDefault(x => x.Code == "AlandIsland");

                    if (alandIslands == null)
                    {
                        Console.WriteLine($"{DateTime.UtcNow} - ERROR: No Aland islands category found.");
                        return;
                    }
                    
                    alandIslands.Code = "Church";
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                });
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
