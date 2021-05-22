using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebDemo.Services;

namespace WebDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly AlertsApiService _alertService;

        public HomeController()
        {
            _alertService = new AlertsApiService();
        }
        public async Task<ActionResult> Index()
        {
            var currentAlerts = await _alertService.GetCurrentAlertsAsync();



            return View();
        }
    }
}