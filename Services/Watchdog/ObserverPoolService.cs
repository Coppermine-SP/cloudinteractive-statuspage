using System.Runtime.InteropServices.JavaScript;
using System.Text;
using static cloudinteractive_statuspage.Services.Configuration;

namespace cloudinteractive_statuspage.Services.Watchdog
{
    
    public class ObserverPoolService
    {
        public const int AUTO_SUMMARY_RATE = 1000000;

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly ConfigService _configService;

        private Dictionary<string, StateObserver> _observerTable;

        private bool _disposed;

        public ObserverPoolService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<ObserverPoolService>>();
            _configService = serviceProvider.GetRequiredService<ConfigService>();

            _observerTable = new Dictionary<string, StateObserver>();
            _disposed = false;
        }

        public void Init()
        {
            _logger.LogInformation("observerPool Started!");
            foreach (var service in _configService.Services)
            {
                _addObserver(_serviceProvider.GetRequiredService<ILogger<StateObserver>>(), service.Url, service.Url, x => _logger.LogError(x.ObserverException, null), EAddressType.DNS);
            }
            foreach (var coreService in _configService.CoreServices)
            {
                string addr = $"{coreService.IP}:{coreService.Port}";
                _addObserver(_serviceProvider.GetRequiredService<ILogger<StateObserver>>(), addr, addr, x => _logger.LogError(x.ObserverException, null), EAddressType.IPv4);
            }

            StartAsync().Wait();
        }

        private ObserverPoolService _addObserver(ILogger logger, string key, string address, StateObserver.ErrorCallback errorCallback, EAddressType addressType)
        {
            if (_observerTable.ContainsKey(key))
                throw new Exception("duplicated key.");
            _observerTable.Add(key, new StateObserver(logger, address, addressType, errorCallback, _configService.ObserverTimeWindow, _configService.TimeoutThreshold));

            return this;
        }

        public StateObserver? GetObserver(string key)
        {
            if (_observerTable.ContainsKey(key)) return _observerTable[key];

            return null;
        }

        public StateObserver[] GetObserverAll() => _observerTable.Values.ToArray();

        public async Task StartAsync()
        {
            if (_disposed) return;
            if (_observerTable.Count == 0) return;

            List<Task> tasks = new List<Task>(_observerTable.Count);
            foreach (var observer in _observerTable.Values)
            {
                tasks.Add(observer.StartAsync());
            }

            await Task.WhenAll(tasks);

            if (_configService.ShowAutoSummary)
            {
                _logger.LogInformation($"Observer auto-summary will show every {AUTO_SUMMARY_RATE}ms.");
                Task.Run(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(AUTO_SUMMARY_RATE);
                        StringBuilder builder = new StringBuilder();
                        builder.AppendLine();
                        builder.AppendLine($"ObserverPool auto-summary - {DateTime.Now.ToString()}:");
                        foreach (var observer in _observerTable.Values)
                        {
                            builder.AppendLine($"Observer #{observer.ThreadId} - {observer.TargetHostName}");
                            builder.AppendLine(
                                $"{observer.TotalTimeWindow} / {observer.FailedTimeWindow} (TotalTimeWindow / FailedTimeWindow)");
                            builder.AppendLine($"Quality = {observer.ServiceQuality}%");
                            builder.AppendLine();
                        }

                        _logger.LogInformation(builder.ToString());
                    }
                });
            }
        }

        public void Release()
        {
            if (_disposed) return;
            foreach (var observer in _observerTable.Values)
            {
                observer.Stop();
            }

            _disposed = true;
            _observerTable.Clear();
        }
    }
}
