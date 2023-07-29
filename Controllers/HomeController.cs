using cloudinteractive_statuspage.Models;
using cloudinteractive_statuspage.Services;
using cloudinteractive_statuspage.Services.Watchdog;
using Microsoft.AspNetCore.Mvc;
using Ng.Services;
using static cloudinteractive_statuspage.Services.Configuration;

namespace cloudinteractive_statuspage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserAgentService _userAgentService;
        private readonly NotifyService _notifyService;
        private readonly ObserverPoolService _observerPoolService;
        private readonly ConfigService _configService;

        public HomeController(ILogger<HomeController> logger, IUserAgentService userAgentService, NotifyService notifyService, ObserverPoolService observerPoolService, ConfigService configService)
        {
            _logger = logger;
            _userAgentService = userAgentService;
            _notifyService = notifyService;
            _observerPoolService = observerPoolService;
            _configService = configService;
        }

        public IActionResult Index()
        {
            var model = new DashboardModel();

            model.NotifyList = DashboardModel.ConvertToNotifyItemList(_notifyService.Notices);

            foreach (var coreService in _configService.CoreServices)
            {
                string addr = $"{coreService.IP}:{coreService.Port}";
                StateObserver? observer = _observerPoolService.GetObserver(addr);

                if (observer != null)
                    model.CoreServiceList.Add(new CoreServiceStateItem(coreService.Name, observer.IsServerOnline));
            }
            foreach (var service in _configService.Services)
            {
                StateObserver? observer = _observerPoolService.GetObserver(service.Url);

                if (observer != null)
                {
                    float sla = (float)Math.Round(observer.ServiceQuality, 2);
                    model.ServiceList.Add(new ServiceStateItem(service.Name, service.SubName, observer.IsServerOnline, service.IsMaintenance, sla));
                }
            }

            model.ConnectionState = _getClientInfo();
            model.Vaildate();
            return View(model);
        }

        private ConnectionStateItem _getClientInfo()
        {
            var _ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var _userAgent = _userAgentService.Parse($"{this.Request?.HttpContext?.Request?.Headers["user-agent"]}");
            return new ConnectionStateItem(_ipAddress, _userAgent);
        }


    }
}