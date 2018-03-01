/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
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
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;
using PTV.Database.DataAccess.Migrations.Base;
using PTV.Database.DataAccess.Utils;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class ServiceLocationChannelServiceAreas : IPartialMigration
    {
        private DataUtils dataUtils;

        public ServiceLocationChannelServiceAreas()
        {
            dataUtils = new DataUtils();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
			dataUtils.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_4_5", "Fixed", "7ServiceLocationChannelServiceAreas.sql"));

            migrationBuilder.DropTable(
                name: "ServiceLocationChannelServiceArea",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "ServiceAreaRestricted",
                schema: "public",
                table: "ServiceLocationChannel");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ServiceAreaRestricted",
                schema: "public",
                table: "ServiceLocationChannel",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ServiceLocationChannelServiceArea",
                schema: "public",
                columns: table => new
                {
                    ServiceLocationChannelId = table.Column<Guid>(nullable: false),
                    MunicipalityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLocationChannelServiceArea", x => new { x.ServiceLocationChannelId, x.MunicipalityId });
                    table.ForeignKey(
                        name: "FK_ServiceLocationChannelServiceArea_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalSchema: "public",
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceLocationChannelServiceArea_ServiceLocationChannel_ServiceLocationChannelId",
                        column: x => x.ServiceLocationChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceLocationChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SerLocChaSerAre_MunicipalityId",
                schema: "public",
                table: "ServiceLocationChannelServiceArea",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_SerLocChaSerAre_ServiceLocationChannelId",
                schema: "public",
                table: "ServiceLocationChannelServiceArea",
                column: "ServiceLocationChannelId");
        }
    }
}
