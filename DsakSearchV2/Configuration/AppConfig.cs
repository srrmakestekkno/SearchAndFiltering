using Microsoft.Extensions.Configuration;

namespace DsakSearchV2.Configuration
{
    public class AppConfig : IDsakConfiguration
    {
        private IConfiguration _configuration;

        public AppConfig(IConfiguration config)
        {
            _configuration = config;
        }
        public string? GetConnectionString()
        {
            var datasource = _configuration.GetValue<string>("Dsak:DataSource");
            var username = _configuration.GetValue<string>("Dsak:Username");
            var password = _configuration.GetValue<string>("Dsak:Password");

            var connectionString = $"Data Source={datasource};User Id={username};Password={password}";
            return connectionString;
        }
    }
}
