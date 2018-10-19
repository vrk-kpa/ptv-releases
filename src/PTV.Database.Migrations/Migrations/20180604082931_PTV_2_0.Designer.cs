using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.ApplicationDbContext;

namespace PTV.Database.Migrations.Migrations
{
    [DbContext(typeof(PtvDbContext))]
    [Migration("20180604082931_PTV_2_0")]
    partial class PTV_2_0
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            migrations.Last().BuildTargetModel(modelBuilder);
        }
    }
}
