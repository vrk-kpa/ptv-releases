using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.V2;

namespace PTV.Database.Migrations.Migrations
{
    public partial class UpdateServiceProcessingInfoDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            this.AddMigrationAction(serviceProvider =>
            {
                var translationService = serviceProvider.GetService<ITranslationService>();
                translationService.UpdateAllWrongServiceProcessingInfo(); //SFIPTV-508
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
