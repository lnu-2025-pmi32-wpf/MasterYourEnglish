namespace MasterYourEnglish.DAL
{
    using System;
    using System.IO;
    using System.Linq;
    using MasterYourEnglish.DAL.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            string currentDir = Directory.GetCurrentDirectory();
            DirectoryInfo directory = new DirectoryInfo(currentDir);

            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }

            if (directory == null)
            {
                throw new DirectoryNotFoundException("Could not find solution root. Make sure a .sln file exists.");
            }

            string startupProjectPath = Path.Combine(directory.FullName, "MasterYourEnglish.Presentation");

            if (!Directory.Exists(startupProjectPath))
            {
                throw new DirectoryNotFoundException($"Could not find startup project path: {startupProjectPath}");
            }

            string settingsPath = Path.Combine(startupProjectPath, "appsettings.json");
            if (!File.Exists(settingsPath))
            {
                throw new FileNotFoundException($"Could not find appsettings.json at: {settingsPath}");
            }

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(startupProjectPath)
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Could not find 'DefaultConnection' in appsettings.json");
            }

            builder.UseNpgsql(connectionString);

            return new ApplicationDbContext(builder.Options);
        }
    }
}