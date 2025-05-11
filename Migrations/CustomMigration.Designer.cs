using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Hostel2._0.Migrations
{
    [DbContext(typeof(Hostel2._0.Data.ApplicationDbContext))]
    [Migration("20250511100000_CustomMigration")]
    partial class CustomMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);
        }
    }
} 