using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.ApplicationDbContext;

namespace PTV.Database.Migrations.Migrations
{
    [DbContext(typeof(PtvDbContext))]
    [Migration("20170925071954_PTV_1_7")]
    partial class PTV_1_7
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            migrations.Last().BuildTargetModel(modelBuilder);
        }
    }
}
