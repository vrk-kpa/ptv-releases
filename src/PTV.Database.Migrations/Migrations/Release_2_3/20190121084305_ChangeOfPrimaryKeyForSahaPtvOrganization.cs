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
using PTV.Database.DataAccess.Interfaces.Services;

namespace PTV.Database.Migrations.Migrations.Release_2_3
{
    public partial class ChangeOfPrimaryKeyForSahaPtvOrganization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            DO $$
            BEGIN
            ALTER TABLE public.""SahaOrganizationInformation"" DROP CONSTRAINT ""PK_SahaOrganizationInformation"";
            DROP INDEX IF EXISTS ""IX_SahOrgInf_OrganizationId"";
            ALTER TABLE public.""SahaOrganizationInformation"" ADD CONSTRAINT ""PK_SahaOrganizationInformation"" PRIMARY KEY (""OrganizationId"", ""SahaId"");
            CREATE INDEX IF NOT EXISTS ""IX_SahOrgInf_OrganizationId_SahaId"" ON public.""SahaOrganizationInformation"" (""OrganizationId"", ""SahaId"");
            END $$");

            // removed from the migration, removal done before this migration
//            this.AddMigrationAction(serviceProvider =>
//            {
//                var migrationService = serviceProvider.GetService<IMigrationService>();
//                migrationService.RemoveOldSahaGuids(); //SFIPTV-923
//            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
