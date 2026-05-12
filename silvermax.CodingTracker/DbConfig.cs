using Microsoft.Extensions.Configuration;

namespace silvermax.CodingTracker;

internal class DbConfig
{
    private readonly string _connectionString;

    public DbConfig()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new Exception("Connection String not found.");
    }

    public string ConnectionString => _connectionString;
}
