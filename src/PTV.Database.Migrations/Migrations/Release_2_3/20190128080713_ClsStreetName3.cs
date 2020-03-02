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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.DataMigrations;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Database.Migrations.Migrations.Release_2_3
{
    public partial class ClsStreetName3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name3",
                schema: "public",
                table: "ClsAddressStreetName",
                maxLength: 3,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClsAddStrNam_Name3",
                schema: "public",
                table: "ClsAddressStreetName",
                column: "Name3");

            this.AddMigrationAction(serviceProvider =>
            {
                var proxyServerSettings = new ProxyServerSettings();
                var clsStreetDownloadSettings = new ClsStreetDownloadSettings();
                var applicationConfiguration = serviceProvider.GetService<ApplicationConfiguration>();
                applicationConfiguration.RawConfiguration.GetSection("ProxyServerSettings").Bind(proxyServerSettings);
                applicationConfiguration.RawConfiguration.GetSection("ClsStreetDownloadSettings").Bind(clsStreetDownloadSettings);
                var clsStreetDownloadTask = new ClsStreetDownload(serviceProvider);
                // var convertAddressesTask = new OldAddressToClsStructConverter(serviceProvider);
                clsStreetDownloadTask.DownloadAddresses();
                // convertAddressesTask.ConvertAddresses();
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClsAddStrNam_Name3",
                schema: "public",
                table: "ClsAddressStreetName");

            migrationBuilder.DropColumn(
                name: "Name3",
                schema: "public",
                table: "ClsAddressStreetName");
        }
    }
}
