using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.TextManager;

namespace PTV.Database.Migrations.Migrations.Release_3_3_1
{
    public partial class UpdateDescriptionsToJson : Migration
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
                    System.Console.WriteLine($@"Update Descriptions is starting......");
                    var deadlineDescriptionTypeId = commonService.GetDescriptionTypeId(DescriptionTypeEnum.DeadLineAdditionalInfo.ToString());
                    var processingTimeTypeId = commonService.GetDescriptionTypeId(DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString());
                    var validityTimeTypeId = commonService.GetDescriptionTypeId(DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString());
                    var chargeTypeTypeId = commonService.GetDescriptionTypeId(DescriptionTypeEnum.ChargeTypeAdditionalInfo.ToString());
                    var descriptionTypeId = commonService.GetDescriptionTypeId(DescriptionTypeEnum.Description.ToString());
                    
                    
                    //Service descriptions
                    updatedItemCount = 0;
                    var serviceDescriptionRep = unitOfWork.CreateRepository<IServiceDescriptionRepository>();
                    serviceDescriptionRep.All().Where(x =>
                              x.TypeId == deadlineDescriptionTypeId || 
                              x.TypeId == processingTimeTypeId || 
                              x.TypeId == validityTimeTypeId || 
                              x.TypeId == chargeTypeTypeId)
                        .ForEach(entity =>
                        {
                            if (!string.IsNullOrEmpty(entity.Description) && !IsValidJson(entity.Description))
                            {
                                entity.Description = textManager.ConvertTextWithLineBreaksToJson(entity.Description);
                                updatedItemCount++;
                            }
                        });
                    System.Console.WriteLine($@"Update ServiceDescriptions is done. Result updated {updatedItemCount}.");
                    
                    //General descriptions
                    updatedItemCount = 0;
                    var statutoryServiceDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceDescriptionRepository>();
                    statutoryServiceDescriptionRep.All().Where(x =>
                            x.TypeId == deadlineDescriptionTypeId || 
                            x.TypeId == processingTimeTypeId || 
                            x.TypeId == validityTimeTypeId || 
                            x.TypeId == chargeTypeTypeId)
                        .ForEach(entity =>
                        {
                            if (!string.IsNullOrEmpty(entity.Description) && !IsValidJson(entity.Description))
                            {
                                entity.Description = textManager.ConvertTextWithLineBreaksToJson(entity.Description);
                                updatedItemCount++;
                            }
                        });
                    System.Console.WriteLine($@"Update GeneralDescriptions is done. Result updated {updatedItemCount}.");
                    
                    //Connections
                    updatedItemCount = 0;
                    var connecionDescriptionRep = unitOfWork.CreateRepository<IServiceServiceChannelDescriptionRepository>();
                    connecionDescriptionRep.All().Where(x =>
                            x.TypeId == descriptionTypeId || 
                            x.TypeId == chargeTypeTypeId)
                        .ForEach(entity =>
                        {
                            if (!string.IsNullOrEmpty(entity.Description) && !IsValidJson(entity.Description))
                            {
                                entity.Description = textManager.ConvertTextWithLineBreaksToJson(entity.Description);
                                updatedItemCount++;
                            }
                        });
                     System.Console.WriteLine($@"Update ServiceServiceChannelDescriptions is done. Result updated {updatedItemCount}.");
                    
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
