using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace TempData.Models
{
    class EFContext : DbContext
    {
        private string connectionString;

        public EFContext() : base()
        {
            var build = new ConfigurationBuilder();
            build.AddJsonFile("appsettings.json", optional: false);
            var config = build.Build();
            connectionString = config.GetConnectionString("sqlConnection");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<Temperature> Temperatures { get; set; }
    }
}
