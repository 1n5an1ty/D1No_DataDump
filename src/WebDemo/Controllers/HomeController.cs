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


        public async Task<ActionResult> Index()
        {            
            return View(await _alertDashboardService.LoadAllAlerts());
        }

        [HttpPost]
        public async Task<ActionResult> Search(RequestFilters filters)
        {
            return Json(await _alertDashboardService.SearcAlerts(filters));
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