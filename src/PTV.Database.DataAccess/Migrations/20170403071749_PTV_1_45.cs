using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class PTV_1_45 : Migration
    {
        readonly List<IPartialMigration> migrations = new List<IPartialMigration>
        {
            new PostOfficeBoxName(),
            new RemoveDescriptionLength(),
            new AreaInformationType(),
            new AreaType(),
            new AreaChanges(),
            new AreaCode(),
            new ServiceChannelConnectionType(),
            new OrganizationDisplayNameType(),
            new LawNameLength500(),
            new ServiceMunicipalityRemove(),
            new ServiceLocationChannelServiceAreas(),
            new BugReport()
        };

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrations.ForEach(m =>
            {
                m.Up(migrationBuilder);
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrations.ForEach(m =>
            {
                m.Down(migrationBuilder);
            });
        }
    }
}
