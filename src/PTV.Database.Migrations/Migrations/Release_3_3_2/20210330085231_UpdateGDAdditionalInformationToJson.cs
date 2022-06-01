using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.TextManager;

namespace PTV.Database.Migrations.Migrations.Release_3_3_2
{
    public partial class UpdateGDAdditionalInformationToJson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            this.AddMigrationAction(serviceProvider =>
            {
                var textManager = new TextManager();
                var contextManager = serviceProvider.GetService<IContextManager>();
                var commonService = serviceProvider.GetService<ICommonService>();
                var updatedItemCount = 0;
                
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    System.Console.WriteLine($@"Update generalDescriptionTypeAdditionalInformation is starting......");
                    var generalDescriptionTypeAdditionalInformationTypeId = commonService.GetDescriptionTypeId(DescriptionTypeEnum.GeneralDescriptionTypeAdditionalInformation.ToString());
                    
                    //General descriptions
                    updatedItemCount = 0;
                    var statutoryServiceDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceDescriptionRepository>();
                    statutoryServiceDescriptionRep.All().Where(x =>
                            x.TypeId == generalDescriptionTypeAdditionalInformationTypeId)
                        .ForEach(entity =>
                        {
                            if (!string.IsNullOrEmpty(entity.Description) && !IsValidJson(entity.Description))
                            {
                                entity.Description = textManager.ConvertTextWithLineBreaksToJson(entity.Description);
                                updatedItemCount++;
                            }
                        });
                    System.Console.WriteLine($@"Update generalDescriptionTypeAdditionalInformation is done. Result updated {updatedItemCount}.");
                    
                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                    System.Console.WriteLine("Done.");
                });
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
        
        private bool IsValidJson(string json)
        {
            return (json.StartsWith("{") && json.EndsWith("}")) || (json.StartsWith("[") && json.EndsWith("]"));
        }
    }
}
