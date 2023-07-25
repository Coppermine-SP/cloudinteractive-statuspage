using cloudinteractive_statuspage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace cloudinteractive_statuspage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var Testmodel = new DashboardModel();
            //Testmodel.NotifyList.Add(new NotifyItem(NotifyItem.NotifyType.Info, "새로운 클라우드인터렉티브 C3 리전에 대해 자세히 알아보십시오."));
            Testmodel.NotifyList.Add(new NotifyItem(NotifyItem.NotifyType.Warn, "한국 동남부지역 기상 악화에 따른 접속 지연 문제 해결 중. (C1)"));
            return View(Testmodel);
        }

    }
}