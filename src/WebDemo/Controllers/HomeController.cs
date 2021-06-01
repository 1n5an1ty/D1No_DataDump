using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDemo.Models;
using WebDemo.Services;

namespace WebDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly AlertDashboardService _alertDashboardService;

        public HomeController()
        {
            _alertDashboardService = new AlertDashboardService();
        }

        public ActionResult Index()
        {            
            return View();
        }

        public async Task<ActionResult> ActiveAlerts()
        {
            var data = new
            {
                data = await _alertDashboardService.LoadAllAlerts()
            };

            return Json(data);
        }

        [HttpPost]
        public async Task<ActionResult> Search(RequestFilters filters)
        {
            var data = new
            {
                data = await _alertDashboardService.SearcAlerts(filters)
            };

            return Json(data);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _alertDashboardService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}