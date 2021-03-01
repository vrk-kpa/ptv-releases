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
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Framework;

namespace PTV.Database.Migrations.Migrations.Release_2_4
{
    public partial class ConnectionsTwoOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceServiceChannel",
                newName: "ServiceOrderNumber");

            migrationBuilder.AddColumn<int>(
                name: "ChannelOrderNumber",
                schema: "public",
                table: "ServiceServiceChannel",
                nullable: true);
            
            this.AddMigrationAction(serviceProvider =>
            {
                var contextManager = serviceProvider.GetService<IContextManager>();
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var connectionsRepo = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
                    connectionsRepo.All().ForEach(connection =>
                    {
                        // Until now, both service order and channel order was mixed in a single column. Since it is
                        // often not clear, which order (service or channel) was stored in the original column,
                        // let's start with the old values in both columns and users can later fix them by themselves.
                        connection.ChannelOrderNumber = connection.ServiceOrderNumber;
                    });

                    unitOfWork.Save(SaveMode.AllowAnonymous);
                });
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelOrderNumber",
                schema: "public",
                table: "ServiceServiceChannel");

            migrationBuilder.RenameColumn(
                name: "ServiceOrderNumber",
                schema: "public",
                table: "ServiceServiceChannel",
                newName: "OrderNumber");
        }
    }
}
