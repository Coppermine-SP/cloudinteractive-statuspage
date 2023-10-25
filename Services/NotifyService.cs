using System.Text.Json.Serialization;
using static cloudinteractive_statuspage.Services.Configuration;

namespace cloudinteractive_statuspage.Services
{
    public class Notify
    {
        public enum NotifyType
        {
            Info = 0,
            Warn = 1,
        }

        public string Content { get; set; }

        public NotifyType Type { get; set; }
    }

    public class NotifyConfig
    {
        public List<Notify> Notices { get; set; }
    }

    public class NotifyService
    {
        private ILogger _logger;
        private IServiceProvider _serviceProvider;
        public NotifyService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetService<ILogger<NotifyService>>();
        }

        public List<Notify> Notices { get; private set; } = new List<Notify>();

        public void LoadFromFile()
        {
            _logger.LogInformation("Load notifications from file..");

            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("notify.json").Build();

                var section = config.GetSection("NotifyConfig");
                var notifyConfig = section.Get<NotifyConfig>();

                _logger.LogInformation($"found {notifyConfig.Notices.Count} notifications from file.");
                Notices = notifyConfig.Notices;
            }
            catch (Exception e)
            {
                _logger.LogWarning("Invalid notification configuration! check notify.json.");
                _logger.LogWarning("NotifyService will NOT load notices from file : " + e);
            }
        }
    }
    
}
