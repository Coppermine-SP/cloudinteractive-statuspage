using cloudinteractive_statuspage.Models;
using cloudinteractive_statuspage.Services.Watchdog;
using static cloudinteractive_statuspage.Services.Configuration;

namespace cloudinteractive_statuspage.Services
{
    public class WatchdogService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        private ConfigService _configService;
        private ServerStateManager? _serverStateManager;

        public ServerStateManager? StateManager => _serverStateManager;

        public WatchdogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<WatchdogService>>();
            _configService = serviceProvider.GetRequiredService<ConfigService>();
        }


        public void Init()
        {
            _logger.LogInformation("Watchdog service init..");
            var config = _serviceProvider.GetService<ConfigService>();

            if(config == null) { return; }

            _serverStateManager = new ServerStateManager(config.PollingRateMs, 1000, 0);

            foreach (var service in config.Services)
            {
                _logger.LogInformation($"service name: {service.Name}, URI: {service.Url}");
                _serverStateManager.AddObserver(service.Url, service.Url, x => _logger.LogError(x.ObserverException, null), EAddressType.DNS);
            }
            foreach (var coreService in config.CoreServices)
            {
                _logger.LogInformation($"CoreService name: {coreService.Name}, IP: {coreService.IP}, Port: {coreService.Port}");
                string addr = $"{coreService.IP}:{coreService.Port}";
                _serverStateManager.AddObserver(addr, addr, x => _logger.LogError(x.ObserverException, null), EAddressType.IPv4);
            }

            _serverStateManager.StartAsync().Wait();
        }

    }
}
