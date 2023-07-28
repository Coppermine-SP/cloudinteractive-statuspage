using cloudinteractive_statuspage.Models;

namespace cloudinteractive_statuspage.Services
{
    public class WatchdogService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        public WatchdogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<WatchdogService>>();
            _logger.LogInformation("Hello, World!");
        }

    }
}
