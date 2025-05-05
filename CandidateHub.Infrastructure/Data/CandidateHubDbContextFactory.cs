using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CandidateHub.Infrastructure.Data
{
    internal class CandidateHubDbContextFactory : IDesignTimeDbContextFactory<CandidateHubDbContext>
    {
        public CandidateHubDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

            var optionsBuilder = new DbContextOptionsBuilder<CandidateHubDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new CandidateHubDbContext(optionsBuilder.Options);
        }
    }
}
