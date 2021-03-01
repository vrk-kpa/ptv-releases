/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.TextManager;

namespace PTV.Database.Migrations.Migrations.Release_3_2
{
    public partial class UpdateDescriptionFieldsToJson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "ServiceServiceChannelDescription",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);
            
            this.AddMigrationAction(serviceProvider =>
            {
                var textManager = new TextManager();
                var contextManager = serviceProvider.GetService<IContextManager>();
                var commonService = serviceProvider.GetService<ICommonService>();
                var updatedItemCount = 0;
                
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    System.Console.WriteLine($@"Update ServiceDescription is starting......");
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
                    System.Console.WriteLine($@"Update ServiceDescription is done. Result updated {updatedItemCount}.");
                    
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
                    System.Console.WriteLine($@"Update GeneralDescription is done. Result updated {updatedItemCount}.");
                    
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
                     System.Console.WriteLine($@"Update ServiceServiceChannelDescription is done. Result updated {updatedItemCount}.");
                    
                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                    System.Console.WriteLine("Done.");
                });
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "ServiceServiceChannelDescription",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
        
        private bool IsValidJson(string json)
        {
            return (json.StartsWith("{") && json.EndsWith("}")) || (json.StartsWith("[") && json.EndsWith("]"));
        }
    }
}
