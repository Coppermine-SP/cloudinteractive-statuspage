using cloudinteractive_statuspage.Models;
using cloudinteractive_statuspage.Services;
using Microsoft.AspNetCore.Mvc;
using Ng.Services;

namespace cloudinteractive_statuspage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserAgentService _userAgentService;
        private readonly NotifyService _notifyService;

        public HomeController(ILogger<HomeController> logger, IUserAgentService userAgentService, NotifyService notifyService)
        {
            _logger = logger;
            _userAgentService = userAgentService;
            _notifyService = notifyService;
        }

        public IActionResult Index()
        {
            var model = TestModelDriver.CreateModel(DashboardModel.ConvertToNotifyItemList(_notifyService.Notices));
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