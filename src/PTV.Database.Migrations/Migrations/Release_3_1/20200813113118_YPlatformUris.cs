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
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.Migrations.Migrations.Release_3_1
{
    public partial class YPlatformUris : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Console.WriteLine($"{DateTime.UtcNow} - Adding new columns for Y platform.");
            
            migrationBuilder.AddColumn<string>(
                name: "ParentYUri",
                schema: "public",
                table: "TargetGroup",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YUri",
                schema: "public",
                table: "TargetGroup",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentYUri",
                schema: "public",
                table: "ServiceClass",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YUri",
                schema: "public",
                table: "ServiceClass",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                schema: "public",
                table: "ProvisionType",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OntologyType",
                schema: "public",
                table: "ProvisionType",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                schema: "public",
                table: "ProvisionType",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentYUri",
                schema: "public",
                table: "ProvisionType",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YCode",
                schema: "public",
                table: "ProvisionType",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YUri",
                schema: "public",
                table: "ProvisionType",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentYUri",
                schema: "public",
                table: "OrganizationType",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YCode",
                schema: "public",
                table: "OrganizationType",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YUri",
                schema: "public",
                table: "OrganizationType",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentYUri",
                schema: "public",
                table: "LifeEvent",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YUri",
                schema: "public",
                table: "LifeEvent",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentYUri",
                schema: "public",
                table: "IndustrialClass",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YUri",
                schema: "public",
                table: "IndustrialClass",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProTyp_ParentId",
                schema: "public",
                table: "ProvisionType",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProvisionType_ProvisionType_ParentId",
                schema: "public",
                table: "ProvisionType",
                column: "ParentId",
                principalSchema: "public",
                principalTable: "ProvisionType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProvisionType_ProvisionType_ParentId",
                schema: "public",
                table: "ProvisionType");

            migrationBuilder.DropIndex(
                name: "IX_ProTyp_ParentId",
                schema: "public",
                table: "ProvisionType");

            migrationBuilder.DropColumn(
                name: "ParentYUri",
                schema: "public",
                table: "TargetGroup");

            migrationBuilder.DropColumn(
                name: "YUri",
                schema: "public",
                table: "TargetGroup");

            migrationBuilder.DropColumn(
                name: "ParentYUri",
                schema: "public",
                table: "ServiceClass");

            migrationBuilder.DropColumn(
                name: "YUri",
                schema: "public",
                table: "ServiceClass");

            migrationBuilder.DropColumn(
                name: "IsValid",
                schema: "public",
                table: "ProvisionType");

            migrationBuilder.DropColumn(
                name: "OntologyType",
                schema: "public",
                table: "ProvisionType");

            migrationBuilder.DropColumn(
                name: "ParentId",
                schema: "public",
                table: "ProvisionType");

            migrationBuilder.DropColumn(
                name: "ParentYUri",
                schema: "public",
                table: "ProvisionType");

            migrationBuilder.DropColumn(
                name: "YCode",
                schema: "public",
                table: "ProvisionType");

            migrationBuilder.DropColumn(
                name: "YUri",
                schema: "public",
                table: "ProvisionType");

            migrationBuilder.DropColumn(
                name: "ParentYUri",
                schema: "public",
                table: "OrganizationType");

            migrationBuilder.DropColumn(
                name: "YCode",
                schema: "public",
                table: "OrganizationType");

            migrationBuilder.DropColumn(
                name: "YUri",
                schema: "public",
                table: "OrganizationType");

            migrationBuilder.DropColumn(
                name: "ParentYUri",
                schema: "public",
                table: "LifeEvent");

            migrationBuilder.DropColumn(
                name: "YUri",
                schema: "public",
                table: "LifeEvent");

            migrationBuilder.DropColumn(
                name: "ParentYUri",
                schema: "public",
                table: "IndustrialClass");

            migrationBuilder.DropColumn(
                name: "YUri",
                schema: "public",
                table: "IndustrialClass");
        }
    }
}
