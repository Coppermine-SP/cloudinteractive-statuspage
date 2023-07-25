using cloudinteractive_statuspage.Models;
using Microsoft.AspNetCore.Mvc;
using Ng.Services;

namespace cloudinteractive_statuspage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserAgentService _userAgentService;

        public HomeController(ILogger<HomeController> logger, IUserAgentService userAgentService)
        {
            _logger = logger;
            _userAgentService = userAgentService;
        }

        public IActionResult Index()
        {
            var model = new DashboardModel();
            model.ConnectionState = _getClientInfo();
            model.NotifyList.Add(new NotifyItem(NotifyItem.NotifyType.Info, "새로운 클라우드인터렉티브 C3 리전에 대해 자세히 알아보십시오."));
            model.NotifyList.Add(new NotifyItem(NotifyItem.NotifyType.Warn, "한국 동남부지역 기상 악화에 따른 접속 지연 문제 해결 중. (C1)"));
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