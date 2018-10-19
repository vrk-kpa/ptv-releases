using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.ApplicationDbContext;

namespace PTV.Database.Migrations.Migrations
{
    [DbContext(typeof(PtvDbContext))]
    [Migration("20180309082931_PTV_1_9")]
    partial class PTV_1_9
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            migrations.Last().BuildTargetModel(modelBuilder);
        }
    }
}
