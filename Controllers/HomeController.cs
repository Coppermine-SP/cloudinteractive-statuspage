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
            var model = TestModelDriver.Model;
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