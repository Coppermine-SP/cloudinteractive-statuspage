using Microsoft.Extensions.Configuration;
namespace cloudinteractive_statuspage.Services
{
    public static class Configuration
    {
        public class CoreService
        {
            public string IP { get; set; }
            public int Port { get; set; }
            public string Name { get; set; }
        }

        public class Service
        {
            public string Name { get; set; }
            public string SubName { get; set; }
            public string Url { get; set; }
            public bool IsMaintenance { get; set; }

        }

        public class ServerConfig
        {
            public List<CoreService> CoreServices { get; set; }
            public List<Service> Services { get; set; }
            public int PollingRate { get; set; }
        }

        public class ConfigService
        {
            private IServiceProvider _serviceProvider;
            private ILogger<ConfigService> _logger;
            public ConfigService(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
                _logger = serviceProvider.GetService<ILogger<ConfigService>>();
            }

            public CoreService[] CoreServices { get; private set; }
            public Service[] Services { get; private set; }

            public void Init()
            {
                _logger.LogInformation("ConfigManager Init..");

                try
                {
                    var config = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("config.json").Build();

                    var section = config.GetSection("ServerConfig");
                    var serverConfig = section.Get<ServerConfig>();

                    _logger.LogInformation($"{serverConfig.CoreServices.Count} CoreServices, {serverConfig.Services.Count} Services configured.");
                    CoreServices = serverConfig.CoreServices.ToArray();
                    Services = serverConfig.Services.ToArray();
                }
                catch (Exception e)
                {
                    _logger.LogWarning("Invalid configuration! check config.json.");
                    throw new ArgumentException("config.json is invalid.", e);
                }
            }
        }
    }
}
